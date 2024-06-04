using VideoCollabServer.Dtos.Movie;

namespace VideoCollabServer.Dtos.Room;

public record RoomProfileDto
{
    public string Id { get; set; }
    public MovieProfileRoomDto? Movie { get; set; }
    public int UsersCount { get; set; }
}