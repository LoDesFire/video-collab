using VideoCollabServer.Data;
using VideoCollabServer.Dtos.Movie;
using VideoCollabServer.Interfaces;
using File = VideoCollabServer.Models.File;

namespace VideoCollabServer.Repositories;

public class FileRepository(ApplicationContext context) : IFileRepository
{
    private ApplicationContext Context { get; } = context;

    public async Task<int?> CreateFileAsync(File file)
    {
        var film = await Context.Files.AddAsync(file);
        await Context.SaveChangesAsync();

        var bob = await Context.Files.FindAsync(film);

        return bob?.Id;
    }
}