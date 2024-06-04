using System.Text.Json.Serialization;

namespace VideoCollab.Core.Domain.DTOs;

public class JanusDto
{
    [JsonPropertyName("janus")] 
    public string? JanusAction { get; set; }

    public string? Transaction { get; set; }
    
};