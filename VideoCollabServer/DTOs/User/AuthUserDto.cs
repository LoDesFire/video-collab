using System.ComponentModel.DataAnnotations;

namespace VideoCollabServer.Dtos.User;

public record AuthUserDto
{
    [Required] 
    public string Username { get; set; } = null!;
    [Required] 
    [MinLength(8)]
    public string Password { get; set; } = null!;
}