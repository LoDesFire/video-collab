
namespace VideoCollabServer.Models;

public class Movie
{
    public enum Statuses
    {
        ReadyToView,
        InQueue,
        Transcoding,
        TranscodingError,
        StartTranscodingError
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Statuses Status { get; set; }
    public List<Link> Links { get; set; } = [];
    public List<File> Files { get; set; } = [];
    public List<User> UsersPinnedMovie { get; set; } = [];
    public List<Room> Rooms { get; set; } = [];
}