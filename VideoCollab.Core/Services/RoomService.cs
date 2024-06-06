using VideoCollab.Core.Domain;
using VideoCollab.Core.Domain.Abstractions;
using VideoCollab.Core.Domain.Models;

namespace VideoCollab.Core.Services;

public class RoomService(
    JanusTextroomService janusTextroomService,
    JanusVideoRoomService janusVideoRoomService,
    IUserRepository userRepository,
    IRoomRepository roomRepository) : IRoomService
{
    public async Task<Result<(string, (string, string, string))>> CreateRoomAsync(string userId, bool @private)
    {
        var roomId = Guid.NewGuid().ToString();
        var secret = Guid.NewGuid().ToString();

        var textRoomResult = await janusTextroomService.CreateRoom(userId, roomId, secret, true);
        var videoRoomResult = await janusVideoRoomService.CreateRoom(userId, roomId, secret, true);
        if (!videoRoomResult.Succeeded || !textRoomResult.Succeeded)
            return Result<(string, (string, string, string))>.Error(
                textRoomResult.Errors.Append("Failed to create a room"));

        var user = await userRepository.GetUserByIdAsync(userId);
        var room = new Room
        {
            Owner = user,
            Private = @private,
            Id = roomId,
            TextRoomSecret = secret,
            VideoOperator = user
        };

        await roomRepository.AddRoomAsync(room);

        var joinRes = await JoinTheRoomAsync(userId, roomId);
        if (joinRes.Succeeded)
            return Result<(string, (string, string, string))>.Ok(
                new ValueTuple<string, (string, string, string)>(roomId, joinRes.Value)
            );


        await DeleteRoomAsync(userId, roomId);
        return Result<(string, (string, string, string))>.Error(joinRes.Errors);
    }

    public async Task<Result<(string, string, string)>> JoinTheRoomAsync(string userId, string roomId)
    {
        var room = await roomRepository.GetRoomJoinedUsersByIdAsync(roomId);
        var user = await userRepository.GetUserByIdAsync(userId);
        if (room == null)
            return Result<(string, string, string)>.Error("Room doesn't exist");

        if (room.JoinedUsers.Contains(user))
            return Result<(string, string, string)>.Error("Already joined");

        var userToken = user.GetRoomToken();
        var videoRoomAllowResult = await janusVideoRoomService.AllowToken(userToken, roomId, room.TextRoomSecret);
        var textRoomAllowResult = await janusTextroomService.AllowToken(userToken, roomId, room.TextRoomSecret);
        if (!videoRoomAllowResult.Succeeded || !textRoomAllowResult.Succeeded)
            return Result<(string, string, string)>.Error(videoRoomAllowResult.Errors.Any()
                ? videoRoomAllowResult.Errors
                : textRoomAllowResult.Errors);

        room.JoinedUsers.Add(user);

        await roomRepository.SaveChangesAsync();

        return Result<(string, string, string)>.Ok(
            new ValueTuple<string, string, string>(userId, user.UserName!, userToken));
    }

    public async Task LeaveFromRoom(string userId, string roomId)
    {
        var user = await userRepository.GetUserConnectedRoomsById(userId);
        var room = await roomRepository.GetRoomByIdAsync(roomId);
        if (room == null)
            return;

        user.ConnectedRoom = null;

        // await janusRoomService.DisallowToken(user.GetRoomToken(), roomId, room.TextRoomSecret);

        await roomRepository.SaveChangesAsync();
    }

    public async Task<Result> DeleteRoomAsync(string userId, string roomId)
    {
        var user = await userRepository.GetUserOwnedRoomsById(userId);

        var room = await roomRepository.GetRoomByIdAsync(roomId);
        if (room == null || !user.OwnedRooms.Contains(room))
            return Result.Error("Forbidden");

        await roomRepository.DeleteRoomAsync(room);

        var textRoomDestroyingRes = await janusTextroomService.DestroyRoom(roomId, room.TextRoomSecret);
        var videoRoomDestroyingRes = await janusVideoRoomService.DestroyRoom(roomId, room.TextRoomSecret);
        if (!videoRoomDestroyingRes.Succeeded || !textRoomDestroyingRes.Succeeded)
            return Result.Error(videoRoomDestroyingRes.Errors.Any()
                ? videoRoomDestroyingRes.Errors
                : textRoomDestroyingRes.Errors);

        return Result.Ok();
    }
}