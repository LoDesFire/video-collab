using VideoCollabServer.Dtos.Movie;
using File = VideoCollabServer.Models.File;

namespace VideoCollabServer.Dtos.Room;

public record RoomProfileDto
{
    public int Id { get; set; }
    public MovieProfileRoomDto? Movie { get; set; }
    public int UsersCount { get; set; } 
}