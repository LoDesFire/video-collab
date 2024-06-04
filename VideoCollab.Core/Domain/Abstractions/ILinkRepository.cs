using VideoCollab.Core.Domain.Models;

namespace VideoCollab.Core.Domain.Abstractions;

public interface ILinkRepository
{
    Task<Result<Link>> CreateLinkAsync(Link link);
}