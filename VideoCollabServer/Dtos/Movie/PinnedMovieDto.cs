using VideoCollabServer.Models;

namespace VideoCollabServer.Dtos.Movie;

using File = VideoCollabServer.Models.File;

public class PinnedMovieDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ImageUrl { get; set; } 
    public string? Description { get; set; }
}