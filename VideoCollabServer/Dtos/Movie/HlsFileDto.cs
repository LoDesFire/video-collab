namespace VideoCollabServer.Dtos.Movie;

public record HlsFileDto
{
    public string ContentType { get; set; } = null!;
    public FileStream Stream { get; set; } = null!;
}