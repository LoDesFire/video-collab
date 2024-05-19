using Microsoft.EntityFrameworkCore;
using VideoCollabServer.Data;
using VideoCollabServer.Dtos;
using VideoCollabServer.Dtos.Room;
using VideoCollabServer.Dtos.User;
using VideoCollabServer.Interfaces;
using VideoCollabServer.Models;

namespace VideoCollabServer.Repositories;

public class RoomRepository(ApplicationContext context, IJanusTextroomService janusTextroomService) : IRoomRepository
{
    public async Task<Result<CreatedRoomDto>> CreateRoomAsync(string userId, bool @private)
    {
        var roomId = Guid.NewGuid().ToString();
        var secret = Guid.NewGuid().ToString();

        var roomResult = await janusTextroomService.CreateRoom(roomId, secret, true);
        if (!roomResult.Succeeded)
            return Result<CreatedRoomDto>.Error(roomResult.Errors.Append("Failed to create a room"));

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
        var allowResult = await janusTextroomService.AllowToken(userToken, roomId, room.TextRoomSecret);
        if (!allowResult.Succeeded)
            return Result<JoinedUserDto>.Error(allowResult.Errors.Append("Failed to add user token"));

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

        // await janusTextroomService.DisallowToken(user.GetRoomToken(), roomId, room.TextRoomSecret);

        await context.SaveChangesAsync();
    }

    public Task<Result> ChangeVideoOperator(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> DeleteRoomAsync(string userId, string roomId)
    {
        var user = await context.Users
            .Include(u => u.OwnedRooms)
            .FirstAsync(u => u.Id == userId);

        var room = await context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
        if (room == null)
            return Result.Error("Room doesn't exist");

        if (!user.OwnedRooms.Contains(room))
            return Result.Error("Forbidden");

        var destroyingRes = await janusTextroomService.DestroyRoom(roomId, room.TextRoomSecret);
        if (!destroyingRes.Succeeded)
            return Result.Error(destroyingRes.Errors);
            
        context.Rooms.Remove(room);
        await context.SaveChangesAsync();
        
        return Result.Ok();
    }
}