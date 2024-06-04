using VideoCollab.Core.Domain.Models;

namespace VideoCollab.Core.Domain.Abstractions;

public interface IMovieRepository
{
    public Task DeleteMovieByIdAsync(int movieId);
    public Task<Movie?> GetMovieLinksByIdAsync(int movieId);
    public Task<IEnumerable<Movie>> GetMovieLinksPinnedMoviesByIdAsync();
    public Task<Movie?> GetMoviePinnedByIdAsync(int movieId);
    public Task<Movie?> GetMovieByIdAsync(int movieId);
    public Task AddMovieAsync(Movie movie);
    public Task SaveChangesAsync();
}