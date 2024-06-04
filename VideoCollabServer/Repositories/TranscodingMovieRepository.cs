using Microsoft.EntityFrameworkCore;
using VideoCollabServer.Data;
using VideoCollabServer.Dtos;
using VideoCollabServer.Interfaces;
using VideoCollabServer.Models;

namespace VideoCollabServer.Repositories;

public class TranscodingMovieRepository(ApplicationContext context): ITranscodingMovieRepository
{
    private ApplicationContext Context { get; } = context;
    
    public async Task<Result> ChangeMovieStatusAsync(int movieId, Movie.Statuses status)
    {
        var movie = await Context.Movies.FirstOrDefaultAsync(m => m.Id == movieId);
    
        if (movie is null)
            return Result.Error("There is no such movie");
        
        movie.Status = status;

        await Context.SaveChangesAsync();
        return Result.Ok();
    }
}