using VideoCollabServer.Data;
using VideoCollabServer.Dtos;
using VideoCollabServer.Interfaces;
using VideoCollabServer.Models;

namespace VideoCollabServer.Repositories;

public class LinkRepository(ApplicationContext context) : ILinkRepository
{
    private ApplicationContext Context { get; } = context;

    public async Task<Result<Link>> CreateLinkAsync(Link link)
    {
        try
        {
            await Context.Links.AddAsync(link);
            await Context.SaveChangesAsync();
        }
        catch (OperationCanceledException)
        {
            return new Result<Link>
            {
                Succeeded = false,
                Errors = new List<string>
                {
                    "Operation cancelled"
                }
            };
        }

        return new Result<Link>
        {
            Succeeded = true,
            Value = link,
        };
    }
}