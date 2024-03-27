namespace VideoCollabServer.Dtos.User;

public record NewUserDto
{
    public string Id { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Token { get; set; } = null!;
}