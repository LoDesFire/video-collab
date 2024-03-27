using System.ComponentModel.DataAnnotations;

namespace VideoCollabServer.Dtos.User;

public record AuthUserDto
{
    [Required] 
    public string UserName { get; set; } = null!;
    [Required] 
    [MinLength(8)]
    public string Password { get; set; } = null!;
}