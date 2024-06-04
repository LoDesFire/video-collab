namespace VideoCollab.Core.Domain.DTOs;

public record JanusTextroomDto
{
    public string Request { get; set; } = null!;
    public string Secret { get; set; } = null!;
    public string Room { get; set; } = null!;
}