using System.ComponentModel.DataAnnotations;
using VideoCollabServer.Models;

namespace VideoCollabServer.Dtos.Movie;

public record CreateMovieDto
{
    public string? TrailerLink { get; set; } = null!;
    public string? PosterLink { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}