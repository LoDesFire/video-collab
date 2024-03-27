using System.ComponentModel.DataAnnotations.Schema;

namespace VideoCollabServer.Models;

public class Stream
{
    public int Id { get; set; }
    public User Owner { get; set; } = null!;
    public Room Room { get; set; } = null!;
}