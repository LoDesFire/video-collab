
using System.ComponentModel.DataAnnotations;

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
    
    [MaxLength(256)]
    public string Name { get; set; } = null!;
    
    [MaxLength(4096)]
    public string? Description { get; set; }
    public Statuses Status { get; set; }
    public List<Link> Links { get; set; } = [];
    
    public List<File> Files { get; set; } = [];
    public List<User> UsersPinnedMovie { get; set; } = [];
    
    public List<Room> Rooms { get; set; } = [];
}