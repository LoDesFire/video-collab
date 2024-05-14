using VideoCollabServer.Dtos;
using VideoCollabServer.Models;

namespace VideoCollabServer.Interfaces;

public interface ILinkRepository
{
    Task<Result<Link>> CreateLinkAsync(Link link);
}