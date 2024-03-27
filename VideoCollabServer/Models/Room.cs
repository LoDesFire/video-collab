using System.ComponentModel.DataAnnotations.Schema;

namespace VideoCollabServer.Models;

public class Room
{
    public int Id { get; set; }
    public User Owner { get; set; } = null!;
    public Link InviteLink { get; set; } = null!;
    public List<User> Users { get; set; } = new();
    public List<Stream> Streams { get; set; } = new();
    public List<Movie> Playlist { get; set; } = new();
}