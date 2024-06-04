using VideoCollab.Core.Domain.Models;

namespace VideoCollab.Core.Domain.Abstractions;

public interface IRoomService
{
    public Task<Result<(string, (string, string, string))>> CreateRoomAsync(string userId, bool @private);
    public Task<Result<(string, string, string)>> JoinTheRoomAsync(string userId, string roomId);
    public Task<Result> DeleteRoomAsync(string userId, string roomId);
    public Task LeaveFromRoom(string userId, string roomId);
}