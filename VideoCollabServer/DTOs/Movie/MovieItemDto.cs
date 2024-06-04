namespace VideoCollabServer.Dtos.Movie;

public record MovieItemDto: PinnedMovieDto
{
    public bool Pinned { get; set; }
}