using VideoCollabServer.Models;

namespace VideoCollabServer.Interfaces;

public interface ILinkRepository
{
    Task<Link?> CreateLinkAsync(Link link);
}