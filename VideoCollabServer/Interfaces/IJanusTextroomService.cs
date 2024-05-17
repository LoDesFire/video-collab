using VideoCollabServer.Dtos;

namespace VideoCollabServer.Interfaces;

public interface IJanusTextroomService
{
    public Task<Result> CreateRoom(string roomId, string secret, bool @private);
    public Task<Result> AllowToken(string token, string roomId);
    public Task<Result> ConnectToPlugin();
}