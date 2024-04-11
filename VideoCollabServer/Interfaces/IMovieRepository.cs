using VideoCollabServer.Dtos.Movie;

namespace VideoCollabServer.Interfaces;

public interface IMovieRepository
{
    Task<CreatedMovieDto?> CreateMovieAsync(CreateMovieDto createMovieDto);
    Task DeleteMovieAsync(int movieId);

    Task<bool> ContainsMovieAsync(int movieId);
    
    
}