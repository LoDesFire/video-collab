namespace VideoCollabServer.Dtos.User;

public record UserRecentCallDto
{
    public string Id { get; set; } = null!;
    public string? UserName { get; set; } = null!;
}