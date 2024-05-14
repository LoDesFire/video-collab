using Microsoft.EntityFrameworkCore;
using VideoCollabServer.Data;
using VideoCollabServer.Dtos;
using VideoCollabServer.Dtos.Movie;
using VideoCollabServer.Interfaces;
using VideoCollabServer.Models;

namespace VideoCollabServer.Repositories;

public class MovieRepository(ApplicationContext context, ILinkRepository linkRepository) : IMovieRepository
{
    private ApplicationContext Context { get; } = context;
    private ILinkRepository LinkRepository { get; } = linkRepository;

    public async Task DeleteMovieAsync(int movieId)
    {
        var movie = await Context.Movies.FindAsync(movieId);

        if (movie == null)
            return;

        Context.Remove(movie);
    }

    public async Task<Result<CreatedMovieDto>> CreateMovieAsync(CreateMovieDto createMovieDto)
    {
        List<Link> movieLinks = [];
        
        if (createMovieDto.TrailerLink != null)
        {
            var trailer = await LinkRepository
                .CreateLinkAsync(new Link
                {
                    Type = LinkType.Trailer,
                    Url = createMovieDto.TrailerLink
                });

            if (trailer.Succeeded)
                movieLinks.Add(trailer.Value!);
        }

        if (createMovieDto.PosterLink != null)
        {
            var poster = await LinkRepository
                .CreateLinkAsync(new Link
                {
                    Type = LinkType.Image,
                    Url = createMovieDto.PosterLink
                });

            if (poster.Succeeded)
                movieLinks.Add(poster.Value!);
        }

        var movie = new Movie
        {
            Description = createMovieDto.Description,
            Links = movieLinks,
            Name = createMovieDto.Name
        };

        try
        {
            await Context.Movies.AddAsync(movie);
            await Context.SaveChangesAsync();
        }
        catch (OperationCanceledException)
        {
            return new Result<CreatedMovieDto>
            {
                Succeeded = false,
                Errors = new List<string>
                {
                    "Operation cancelled"
                }
            };
        }
        
        return new Result<CreatedMovieDto>
        {
            Succeeded = true,
            Value = new CreatedMovieDto
            {
                Id = movie.Id
            }
        };
    }

    public async Task<bool> ContainsMovieAsync(int movieId)
    {
        return await Context.Movies.FirstOrDefaultAsync(m => m.Id == movieId) != null;
    }
}