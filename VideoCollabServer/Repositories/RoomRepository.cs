using System.Security.Cryptography;
using System.Text;
using VideoCollabServer.Data;
using VideoCollabServer.Dtos;
using VideoCollabServer.Dtos.Room;
using VideoCollabServer.Dtos.User;
using VideoCollabServer.Interfaces;
using VideoCollabServer.Models;

namespace VideoCollabServer.Repositories;

public class RoomRepository(ApplicationContext context, IJanusTextroomService janusTextroomService): IRoomRepository
{
    public async Task<Result<CreatedRoomDto>> CreateRoomAsync(string userId, bool @private)
    {
        var roomId = Guid.NewGuid().ToString();
        var secret = Guid.NewGuid().ToString();
        
        await janusTextroomService.ConnectToPlugin();
        var roomResult = await janusTextroomService.CreateRoom(roomId, secret, true);
        if (!roomResult.Succeeded)
            return Result<CreatedRoomDto>.Error(roomResult.Errors.Append("Failed to create a room"));
        
        var user = await context.Users.FindAsync(userId);
        var room = new Room
        {
            Owner = user!,
            Private = @private,
            Id = roomId,
            TextRoomSecret = secret,
            VideoOperator = user!
        };
        await context.Rooms.AddAsync(room);
        await context.SaveChangesAsync();
        
        var inputBytes = Encoding.ASCII.GetBytes(user!.UserName! + userId);
        var userToken = Convert.ToHexString(MD5.HashData(inputBytes));
        await janusTextroomService.AllowToken(userToken, roomId);
        
        return Result<CreatedRoomDto>.Ok(
            new CreatedRoomDto
            {
                Id = roomId,
                Owner = new JoinedUserDto
                {
                    Id = userId,
                    Username = user.UserName!,
                    TextroomToken = userToken
                }
            }
            );
    }
    
    public async Task<Result<JoinedUserDto>> JoinTheRoomAsync(string userId, string roomId)
    {
        var room = await context.Rooms.FindAsync(roomId);
        if (room == null)
            return Result<JoinedUserDto>.Error("Room doesn't exist");
        
        var user = await context.Users.FindAsync(userId);
        
        var inputBytes = Encoding.ASCII.GetBytes(user!.UserName! + userId);
        var userToken = Convert.ToHexString(MD5.HashData(inputBytes));
        await janusTextroomService.ConnectToPlugin();
        var allowResult = await janusTextroomService.AllowToken(userToken, roomId);

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

    public Task LeaveFromRoom(int roomId)
    {
        throw new NotImplementedException();
    }

    public Task<Result> ChangeVideoOperator(string userId)
    {
        throw new NotImplementedException();
    }
}