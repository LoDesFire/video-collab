using Microsoft.EntityFrameworkCore;
using VideoCollab.Core.Domain.Abstractions;
using VideoCollab.Core.Domain.Models;
using VideoCollab.Persistence.Data;

namespace VideoCollab.Persistence.Repositories;

public class RoomRepository(ApplicationContext context) : IRoomRepository
{
    public async Task AddRoomAsync(Room room)
    {
        await context.Rooms.AddAsync(room);
        await context.SaveChangesAsync();
    }

    public async Task DeleteRoomAsync(Room room)
    {
        context.Rooms.Remove(room);
        await context.SaveChangesAsync();
    }

    public async Task<Room?> GetRoomJoinedUsersByIdAsync(string roomId)
    {
        return await context.Rooms
            .Include(r => r.JoinedUsers)
            .FirstOrDefaultAsync(r => r.Id == roomId);
    }

    public async Task<Room?> GetRoomByIdAsync(string roomId)
    {
        return await context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}