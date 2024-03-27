using Microsoft.AspNetCore.Identity;

namespace VideoCollabServer.Models;

public class User : IdentityUser
{
    public List<Movie> PinnedMovies { get; set; } = new();
    public List<User> RecentCallUsers { get; set; } = new();
    public List<Room> ConnectedRooms { get; set; } = new();
    public List<Room> OwnedRooms { get; set; } = new();
}