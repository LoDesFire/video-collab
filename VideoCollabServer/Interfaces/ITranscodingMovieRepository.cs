using VideoCollabServer.Dtos;
using VideoCollabServer.Models;

namespace VideoCollabServer.Interfaces;

public interface ITranscodingMovieRepository
{
    public Task<Result> ChangeMovieStatusAsync(int movieId, Movie.Statuses status);
}