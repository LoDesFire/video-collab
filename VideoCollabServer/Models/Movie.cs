
namespace VideoCollabServer.Models;

public class Movie
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public List<Link> Links { get; set; } =  new();
    public List<File> Files { get; set; } =  new();
    public List<User> UsersPinnedMovie { get; set; } =  new();
    public List<Room> Rooms { get; set; } =  new();
}