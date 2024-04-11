namespace VideoCollabServer.Dtos.Movie;

public record PlaylistDto
{
    public bool Exists { get; set; }
    public string? Error { get; set; }
    public FileStream? Stream { get; set; }
}