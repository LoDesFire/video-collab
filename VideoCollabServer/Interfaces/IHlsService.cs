using VideoCollabServer.Dtos;
using VideoCollabServer.Dtos.Movie;

namespace VideoCollabServer.Interfaces;

public interface IHlsService
{
    public Result<FileStream> GetPlaylistByMovieId(int movieId);

    public Result<HlsFileDto> GetHlsFile(int movieId, string quality, string file);
    
    public Task<Result> UploadMovieAsync(int movieId, IFormFile file, IMovieRepository movieRepository);
    
    public void DeleteMovie(int movieId);
}