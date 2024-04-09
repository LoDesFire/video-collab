using VideoCollabServer.Dtos.Movie;
using File = VideoCollabServer.Models.File;

namespace VideoCollabServer.Interfaces;

public interface IFileRepository
{
    Task<int?> CreateFileAsync(File file);
}