using Microsoft.EntityFrameworkCore;
using VideoCollabServer.Data;
using VideoCollabServer.Dtos;
using VideoCollabServer.Dtos.Room;
using VideoCollabServer.Dtos.User;
using VideoCollabServer.Interfaces;
using VideoCollabServer.Models;
using VideoCollabServer.Services;

namespace VideoCollabServer.Repositories;

public class RoomRepository(ApplicationContext context, JanusTextroomService janusTextroomService, JanusVideoRoomService janusVideoRoomService) : IRoomRepository
{
    public async Task<Result<CreatedRoomDto>> CreateRoomAsync(string userId, bool @private)
    {
        var roomId = Guid.NewGuid().ToString();
        var secret = Guid.NewGuid().ToString();

        var textRoomResult = await janusTextroomService.CreateRoom(userId, roomId, secret, true);
        var videoRoomResult = await janusVideoRoomService.CreateRoom(userId, roomId, secret, true);
        if (!videoRoomResult.Succeeded || !textRoomResult.Succeeded)
            return Result<CreatedRoomDto>.Error(textRoomResult.Errors.Append("Failed to create a room"));

        var user = await context.Users.FirstAsync(u => u.Id == userId);
        var room = new Room
        {
            Owner = user,
            Private = @private,
            Id = roomId,
            TextRoomSecret = secret,
            VideoOperator = user
        };
        await context.Rooms.AddAsync(room);
        await context.SaveChangesAsync();
        
        var joinRes = await JoinTheRoomAsync(userId, roomId);
        if (joinRes.Succeeded)
            return Result<CreatedRoomDto>.Ok(
                new CreatedRoomDto
                {
                    Id = roomId,
                    Owner = joinRes.Value!
                }
            );

        await DeleteRoomAsync(userId, roomId);
        return Result<CreatedRoomDto>.Error(joinRes.Errors);
    }

    public async Task<Result<JoinedUserDto>> JoinTheRoomAsync(string userId, string roomId)
    {
        var room = await context.Rooms
            .Include(r => r.JoinedUsers)
            .FirstOrDefaultAsync(r => r.Id == roomId);
        var user = await context.Users.FirstAsync(u => u.Id == userId);
        if (room == null)
            return Result<JoinedUserDto>.Error("Room doesn't exist");

        if (room.JoinedUsers.Contains(user))
            return Result<JoinedUserDto>.Error("Already joined");

        var userToken = user.GetRoomToken();
        var videoRoomAllowResult = await janusVideoRoomService.AllowToken(userToken, roomId, room.TextRoomSecret);
        var textRoomAllowResult = await janusTextroomService.AllowToken(userToken, roomId, room.TextRoomSecret);
        if (!videoRoomAllowResult.Succeeded || !textRoomAllowResult.Succeeded)
            return Result<JoinedUserDto>.Error(videoRoomAllowResult.Errors.Any() ? videoRoomAllowResult.Errors : textRoomAllowResult.Errors);

        room.JoinedUsers.Add(user);

        await context.SaveChangesAsync();

        return Result<JoinedUserDto>.Ok(
            new JoinedUserDto
            {
                Id = userId,
                Username = user.UserName!,
                TextroomToken = userToken
            }
        );
    }

    public async Task LeaveFromRoom(string userId, string roomId)
    {
        var user = await context.Users
            .Include(u => u.ConnectedRooms)
            .FirstAsync(u => u.Id == userId);
        var room = await context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
        if (room == null)
            return;

        user.ConnectedRooms.Remove(room);

        // await janusRoomService.DisallowToken(user.GetRoomToken(), roomId, room.TextRoomSecret);

        await context.SaveChangesAsync();
    }

    public async Task<Result> DeleteRoomAsync(string userId, string roomId)
    {
        var user = await context.Users
            .Include(u => u.OwnedRooms)
            .FirstAsync(u => u.Id == userId);

        var room = await context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
        if (room == null || !user.OwnedRooms.Contains(room))
            return Result.Error("Forbidden");
        
        context.Rooms.Remove(room);
        await context.SaveChangesAsync();
        
        var textRoomDestroyingRes = await janusTextroomService.DestroyRoom(roomId, room.TextRoomSecret);
        var videoRoomDestroyingRes = await janusVideoRoomService.DestroyRoom(roomId, room.TextRoomSecret);
        if (!videoRoomDestroyingRes.Succeeded || !textRoomDestroyingRes.Succeeded)
            return Result.Error(videoRoomDestroyingRes.Errors.Any() ? videoRoomDestroyingRes.Errors : textRoomDestroyingRes.Errors);
        
        return Result.Ok();
    }
}