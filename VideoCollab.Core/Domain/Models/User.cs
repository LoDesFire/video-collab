using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace VideoCollab.Core.Domain.Models;

public class User : IdentityUser
{
    public List<Movie> PinnedMovies { get; init; } = [];
    public List<User> RecentCallUsers { get; init; } = [];
    public List<Room> ConnectedRooms { get; init; } = [];
    public List<Room> OwnedRooms { get; init; } = [];
    public string GetRoomToken()
    {
        var inputBytes = Encoding.ASCII.GetBytes(Id + UserName);
        
        return Convert.ToHexString(MD5.HashData(inputBytes)).ToLower();
    }
}