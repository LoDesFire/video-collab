using VideoCollab.Core.Domain.Models;

namespace VideoCollab.Core.Domain.Abstractions;

public interface IRoomRepository
{
    public Task AddRoomAsync(Room room);
    public Task DeleteRoomAsync(Room room);
    public Task<Room?> GetRoomJoinedUsersByIdAsync(string roomId);
    public Task<Room?> GetRoomByIdAsync(string roomId);
    public Task SaveChangesAsync();
}