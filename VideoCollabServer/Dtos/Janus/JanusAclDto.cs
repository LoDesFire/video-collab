namespace VideoCollabServer.Dtos;

public record JanusAclDto
{
    public string Textroom { get; set; } = null!;
    
    public string Action { get; set; } = null!;
    
    public string Secret { get; set; } = null!;
    
    public string Room { get; set; } = null!;
    
    public IEnumerable<string>? Allowed { get; set; }
    
}