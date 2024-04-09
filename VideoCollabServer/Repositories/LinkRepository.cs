using VideoCollabServer.Data;
using VideoCollabServer.Interfaces;
using VideoCollabServer.Models;

namespace VideoCollabServer.Repositories;

public class LinkRepository: ILinkRepository
{
    private ApplicationContext Context { get; set; }
    
    public LinkRepository(ApplicationContext context)
    {
        Context = context;
    }
    
    public async Task<Link?> CreateLinkAsync(Link link)
    {
        await Context.Links.AddAsync(link);
        await Context.SaveChangesAsync();
        
        return link;
    }
}