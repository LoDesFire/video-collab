using System.Text.Json.Serialization;

namespace VideoCollabServer.Dtos;

public class JanusCreateTextroomDto
{
    public string Request { get; set; } = null!;

    [JsonPropertyName("admin_key")] 
    public string AdminKey { get; set; } = null!;
    
    public string Secret { get; set; } = null!;
    
    public string Room { get; set; } = null!;
    
    public bool Private { get; set; } = true;
    
}