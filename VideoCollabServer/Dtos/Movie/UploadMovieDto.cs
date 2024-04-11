namespace VideoCollabServer.Dtos.Movie;

public record UploadMovieDto
{
    public string? Error { get; set; }
    public bool Succeeded { get; set; }
}   