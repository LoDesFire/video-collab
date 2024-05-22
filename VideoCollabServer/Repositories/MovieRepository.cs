using Microsoft.EntityFrameworkCore;
using VideoCollabServer.Data;
using VideoCollabServer.Dtos;
using VideoCollabServer.Dtos.Movie;
using VideoCollabServer.Interfaces;
using VideoCollabServer.Mappers;
using VideoCollabServer.Models;

namespace VideoCollabServer.Repositories;

public class MovieRepository(ApplicationContext context, ILinkRepository linkRepository) : IMovieRepository
{
    private ApplicationContext Context { get; } = context;
    private ILinkRepository LinkRepository { get; } = linkRepository;

    public async Task DeleteMovieAsync(int movieId)
    {
        var movie = await Context.Movies
            .Include(m => m.Links)
            .Include(m => m.Files)
            .FirstOrDefaultAsync(m => m.Id == movieId);
        if (movie == null)
            return;
        
        movie.Links.Clear();
        movie.Files.Clear();
        Context.Movies.Remove(movie);

        await Context.SaveChangesAsync();
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
            Name = createMovieDto.Name,
            Status = Movie.Statuses.NotUploaded
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

    public async Task<Result<IEnumerable<PinnedMovieDto>>> ReadyToViewMoviesAsync()
    {
        return Result<IEnumerable<PinnedMovieDto>>.Ok(
            await Context.Movies
                .Include(m => m.Links)
                .Where(m => m.Status == Movie.Statuses.ReadyToView)
                .Select(m => m.ToPinnedDto())
                .ToListAsync()
        );
    }
}