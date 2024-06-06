namespace VideoCollabServer.Dtos.Room;

public record RoomProfileDto
{
    public string Id { get; set; }
    public int? Movie { get; set; } = null;
    public int UsersCount { get; set; }
}