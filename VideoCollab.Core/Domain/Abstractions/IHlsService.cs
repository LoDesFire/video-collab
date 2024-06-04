namespace VideoCollab.Core.Domain.Abstractions;

public interface IHlsService
{
    public Result<FileStream> GetPlaylistByMovieId(int movieId);

    public Result<(string, FileStream)> GetHlsFile(int movieId, string quality, string file);
    
    public Task<Result> UploadMovieAsync(int movieId, Stream stream, string fileName, IMovieService movieService);
    
    public void DeleteMovie(int movieId);
}