using VideoCollab.Core.Domain.Models;

namespace VideoCollab.Core.Domain.Abstractions;

public interface IMovieService
{
    Task<Result<Movie>> CreateMovieAsync(string? trailerLink, string? posterLink, string name, string? description);
    Task DeleteMovieAsync(int movieId);
    Task<Result<IEnumerable<Movie>>> ReadyToViewMoviesAsync();
    Task<Result<Movie>> MovieById(int movieId);
    Task<bool> ContainsMovieAsync(int movieId);
    Task<Result> ChangeMovieStatusAsync(int movieId, Movie.Statuses status);
}