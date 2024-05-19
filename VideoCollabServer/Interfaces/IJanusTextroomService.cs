using VideoCollabServer.Dtos;

namespace VideoCollabServer.Interfaces;

public interface IJanusTextroomService
{
    public Task<Result> CreateRoom(string userId, string roomId, string secret, bool @private);
    public Task<Result> AllowToken(string token, string roomId, string secret);
    public Task<Result> DisallowToken(string token, string roomId, string secret);
    public Task<Result> DestroyRoom(string roomId, string secret);
}