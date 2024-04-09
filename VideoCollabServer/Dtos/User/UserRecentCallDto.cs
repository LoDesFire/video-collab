namespace VideoCollabServer.Dtos.User;

public record UserRecentCallDto
{
    public string Id { get; set; } = null!;
    public string? Username { get; set; } = null!;
}