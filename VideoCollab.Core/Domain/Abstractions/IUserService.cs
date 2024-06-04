using VideoCollab.Core.Domain.Models;

namespace VideoCollab.Core.Domain.Abstractions;

public interface IUserService
{
    public Task<User?> GetByIdAsync(string id);
    public Task<bool> PinMovieAsync(string id, int movieId);
    public Task<bool> UnpinMovieAsync(string id, int movieId);
}