using VideoCollab.Core.Domain;
using VideoCollab.Core.Domain.Abstractions;
using VideoCollab.Core.Domain.Models;

namespace VideoCollab.Core.Services;

public class MovieService(
    ILinkRepository linkRepository,
    IMovieRepository movieRepository) : IMovieService
{
    public async Task DeleteMovieAsync(int movieId)
    {
        await movieRepository.DeleteMovieByIdAsync(movieId);
    }

    public async Task<Result<Movie>> CreateMovieAsync(string? trailerLink, string? posterLink, string name,
        string? description)
    {
        List<Link> movieLinks = [];

        if (trailerLink != null)
        {
            var trailer = await linkRepository
                .CreateLinkAsync(new Link
                {
                    Type = LinkType.Trailer,
                    Url = trailerLink
                });

            if (trailer.Succeeded)
                movieLinks.Add(trailer.Value!);
        }

        if (posterLink != null)
        {
            var poster = await linkRepository
                .CreateLinkAsync(new Link
                {
                    Type = LinkType.Image,
                    Url = posterLink
                });

            if (poster.Succeeded)
                movieLinks.Add(poster.Value!);
        }

        var movie = new Movie
        {
            Description = description,
            Links = movieLinks,
            Name = name,
            Status = Movie.Statuses.NotUploaded
        };

        await movieRepository.AddMovieAsync(movie);

        return new Result<Movie>
        {
            Succeeded = true,
            Value = movie
        };
    }

    public async Task<Result<Movie>> MovieById(int movieId)
    {
        var movie = await movieRepository.GetMovieLinksByIdAsync(movieId);

        return movie == null
            ? Result<Movie>.Error("Movie doesn't exists")
            : Result<Movie>.Ok(movie);
    }

    public async Task<bool> ContainsMovieAsync(int movieId)
    {
        return await movieRepository.GetMovieByIdAsync(movieId) != null;
    }

    public async Task<Result> ChangeMovieStatusAsync(int movieId, Movie.Statuses status)
    {
        var movie = await movieRepository.GetMovieByIdAsync(movieId);

        if (movie is null)
            return Result.Error("There is no such movie");

        movie.Status = status;

        await movieRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<IEnumerable<Movie>>> ReadyToViewMoviesAsync()
    {
        return Result<IEnumerable<Movie>>.Ok(
                await movieRepository.GetMovieLinksPinnedMoviesByIdAsync()
            );
    }
}