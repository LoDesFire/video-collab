namespace VideoCollabServer.Models;

public record AuthResult
{
    public int StatusCode { get; set; }
    public bool Succeeded { get; set; } 
    public string Token { get; set; } = null!;
    public IEnumerable<string> Errors { get; set; } = null!;
}