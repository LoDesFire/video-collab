namespace VideoCollabServer.Dtos.Movie;

public record HlsFileDto
{
    public string? ContentType { get; set; }
    public FileStream? Stream { get; set; }
    public bool Succeeded;
}