using VideoCollab.Core.Domain.Models;
using VideoCollabServer.Dtos.Room;

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