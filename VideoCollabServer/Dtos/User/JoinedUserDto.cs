namespace VideoCollabServer.Dtos.User;

public record JoinedUserDto
{
    public string Id { get; set; } = null!;
    public string TextroomToken { get; set; } = null!;
    public string Username { get; set; } = null!;
}