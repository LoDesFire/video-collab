using Microsoft.EntityFrameworkCore;
using VideoCollab.Core.Domain.Abstractions;
using VideoCollab.Core.Domain.Models;
using VideoCollab.Persistence.Data;

namespace VideoCollab.Persistence.Repositories;

public class MovieRepository(ApplicationContext context) : IMovieRepository
{
    public async Task DeleteMovieByIdAsync(int movieId)
    {
        var movie = await context.Movies
            .Include(m => m.Links)
            .FirstOrDefaultAsync(m => m.Id == movieId);
        if (movie == null)
            return;

        movie.Links.Clear();
        context.Movies.Remove(movie);

        await context.SaveChangesAsync();
    }

    public async Task<Movie?> GetMovieLinksByIdAsync(int movieId)
    {
        return await context.Movies
            .Include(m => m.Links)
            .FirstOrDefaultAsync(m => m.Id == movieId);
    }
    
    public async Task<Movie?> GetMoviePinnedByIdAsync(int movieId)
    {
        return await context.Movies
            .Include(m => m.UsersPinnedMovie)
            .FirstOrDefaultAsync(m => m.Id == movieId);
    }

    public async Task<IEnumerable<Movie>> GetMovieLinksPinnedMoviesByIdAsync()
    {
        return await context.Movies
            .Include(m => m.Links)
            .Include(m => m.UsersPinnedMovie)
            .Where(m => m.Status == Movie.Statuses.ReadyToView)
            .ToListAsync();
    }

    public async Task<Movie?> GetMovieByIdAsync(int movieId)
    {
        return await context.Movies.FirstOrDefaultAsync(m => m.Id == movieId);
    }

    public async Task AddMovieAsync(Movie movie)
    {
        await context.Movies.AddAsync(movie);
        await context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}