using VideoCollabServer.Data;
using VideoCollabServer.Dtos.Room;
using VideoCollabServer.Models;

namespace VideoCollabServer.Mappers;

public static class RoomMappers
{
    public static RoomProfileDto ToProfileDto(this Room roomModel)
    {
        return new RoomProfileDto
        {
            Id = roomModel.Id,
            UsersCount = roomModel.JoinedUsers.Count,
            Movie = roomModel.Playlist.FirstOrDefault()?.ToProfileRoomDto(),
        };
    }
}