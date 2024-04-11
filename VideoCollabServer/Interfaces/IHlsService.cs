using VideoCollabServer.Dtos.Movie;

namespace VideoCollabServer.Interfaces;

public interface IHlsService
{
    public PlaylistDto GetPlaylistByMovieId(int movieId);

    public HlsFileDto GetHlsFile(int movieId, string quality, string file);
    
    public Task<UploadMovieDto> UploadMovieAsync(int movieId, IFormFile file, IMovieRepository movieRepository);
    
    public void DeleteMovie(int movieId);
}