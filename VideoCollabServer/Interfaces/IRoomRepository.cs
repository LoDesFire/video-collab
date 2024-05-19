using VideoCollabServer.Dtos;
using VideoCollabServer.Dtos.Room;
using VideoCollabServer.Dtos.User;

namespace VideoCollabServer.Interfaces;

public interface IRoomRepository
{
    public Task<Result<CreatedRoomDto>> CreateRoomAsync(string userId, bool @private);

    public Task<Result<JoinedUserDto>> JoinTheRoomAsync(string userId, string roomId);
    
    public Task<Result> DeleteRoomAsync(string userId, string roomId);
    
    public Task LeaveFromRoom(string userId, string roomId);

    public Task<Result> ChangeVideoOperator(string userId);
}