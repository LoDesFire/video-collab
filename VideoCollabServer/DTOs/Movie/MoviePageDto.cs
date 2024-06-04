namespace VideoCollabServer.Dtos.Movie;

public class MoviePageDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string? TrailerUrl { get; set; }
    public string? Description { get; set; }
}