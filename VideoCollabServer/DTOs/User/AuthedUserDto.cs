namespace VideoCollabServer.Dtos.User;

public record AuthedUserDto
{
    // public string Id { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Token { get; set; } = null!;
}