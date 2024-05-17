using VideoCollabServer.Models;

namespace VideoCollabServer.Dtos.Movie;


public class PinnedMovieDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ImageUrl { get; set; } 
    public string? Description { get; set; }
}