using System.ComponentModel.DataAnnotations;

namespace VideoCollabServer.Models;

public class Room
{
    [MaxLength(256)]
    [Key]
    public string Id { get; set; } = null!;
    public bool Private { get; set; }
    public User Owner { get; set; } = null!;
    public User VideoOperator { get; set; } = null!;
    [MaxLength(256)]
    public string TextRoomSecret { get; set; } = null!;
    public List<User> JoinedUsers { get; init; } = [];
    public List<Movie> Playlist { get; init; } = [];
}

