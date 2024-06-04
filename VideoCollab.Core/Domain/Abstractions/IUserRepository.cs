using VideoCollab.Core.Domain.Models;

namespace VideoCollab.Core.Domain.Abstractions;

public interface IUserRepository
{
   public Task<User> GetUserProfileByIdAsync(string id);
   public Task<User> GetUserConnectedRoomsById(string id);
   public Task<User> GetUserOwnedRoomsById(string id);
   public Task<User> GetUserPinnedMoviesByIdAsync(string id);
   public Task<User> GetUserByIdAsync(string id);
   public Task SaveChangesAsync();
}