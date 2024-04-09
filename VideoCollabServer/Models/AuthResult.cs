using VideoCollabServer.Dtos.User;

namespace VideoCollabServer.Models;

public record AuthResult
{
    public bool Succeeded { get; set; } 
    public AuthedUserDto AuthedUserDto { get; set; } = null!;
    public IEnumerable<string> Errors { get; set; } = null!;
}