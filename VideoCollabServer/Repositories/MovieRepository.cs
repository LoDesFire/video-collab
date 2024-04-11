using Microsoft.EntityFrameworkCore;
using VideoCollabServer.Data;
using VideoCollabServer.Dtos.Movie;
using VideoCollabServer.Interfaces;
using VideoCollabServer.Models;

namespace VideoCollabServer.Repositories;

public class MovieRepository : IMovieRepository
{
    private ApplicationContext Context { get; set; }
    private ILinkRepository LinkRepository { get; set; }

    public MovieRepository(ApplicationContext context, ILinkRepository linkRepository)
    {
        Context = context;
        LinkRepository = linkRepository;
    }

    public async Task<CreatedMovieDto?> CreateMovieAsync(CreateMovieDto createMovieDto)
    {
        List<Link> movieLinks = new();
        if (createMovieDto.TrailerLink != null)
        {
            var trailer = await LinkRepository
                .CreateLinkAsync(new Link
                {
                    Type = LinkType.Trailer,
                    Url = createMovieDto.TrailerLink
                });

            if (trailer != null)
                movieLinks.Add(trailer);
        }

        if (createMovieDto.PosterLink != null)
        {
            var poster = await LinkRepository
                .CreateLinkAsync(new Link
                {
                    Type = LinkType.Image,
                    Url = createMovieDto.PosterLink
                });

            if (poster != null)
                movieLinks.Add(poster);
        }

        var movie = new Movie
        {
            Description = createMovieDto.Description,
            Links = movieLinks,
            Name = createMovieDto.Name
        };

        await Context.Movies.AddAsync(movie);
        await Context.SaveChangesAsync();

        return new CreatedMovieDto
        {
            Id = movie.Id
        };
    }
    
    public async Task<bool> ContainsMovieAsync(int movieId)
    {
        return (await Context.Movies.FirstAsync(m => m.Id == movieId)) != null;
    }
    
}