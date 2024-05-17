using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace VideoCollabServer.Dtos;

public class JanusDto
{
    [JsonPropertyName("janus")] 
    public string? JanusAction { get; set; }

    public string? Transaction { get; set; }
    
};