using Microsoft.EntityFrameworkCore;
using VideoCollab.Core.Domain.Abstractions;
using VideoCollab.Core.Domain.Models;
using VideoCollab.Persistence.Data;

namespace VideoCollab.Persistence.Repositories;

public class UserRepository(ApplicationContext context) : IUserRepository
{
    public async Task<User> GetUserProfileByIdAsync(string id)
    {
        return await context.Users
            .Include(u => u.PinnedMovies)
            .ThenInclude(m => m.Links)
            .Include(u => u.OwnedRooms)
            .ThenInclude(r => r.JoinedUsers)
            .FirstAsync(u => u.Id == id);
    }

    public async Task<User> GetUserConnectedRoomsById(string id)
    {
        return await context.Users
            .FirstAsync(u => u.Id == id);
    }

    public async Task<User> GetUserOwnedRoomsById(string id)
    {
        return await context.Users
            .Include(u => u.OwnedRooms)
            .FirstAsync(u => u.Id == id);
    }

    public async Task<User> GetUserPinnedMoviesByIdAsync(string id)
    {
        return await context.Users
            .Include(u => u.PinnedMovies)
            .FirstAsync(u => u.Id == id);
    }

    public async Task<User> GetUserByIdAsync(string id)
    {
        return await context.Users.FirstAsync(u => u.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}