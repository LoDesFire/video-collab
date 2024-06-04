using VideoCollab.Core.Domain;
using VideoCollab.Core.Domain.Abstractions;
using VideoCollab.Core.Domain.DTOs;

namespace VideoCollab.Core.Services;

public class JanusVideoRoomService : IJanusRoomService
{
    private readonly IJanusPluginService _janusPluginService;

    public JanusVideoRoomService(IJanusPluginService janusPluginService)
    {
        _janusPluginService = janusPluginService;
        _janusPluginService.PluginName = "videoroom";
    }

    public async Task<Result> CreateRoom(string userId, string roomId, string secret, bool @private)
    {
        var res = await _janusPluginService.HandlePluginResponse(
            new JanusCreateVideoRoomDto
            {
                Request = "create",
                AdminKey = _janusPluginService.Secret,
                Room = roomId,
                Private = @private,
                Secret = secret
            }
        );
        
        if (!res.Succeeded)
            return Result.Error(res.Errors);

        return await ActivateAcl(roomId, secret);
    }
        
    private async Task<Result> ActivateAcl(string roomId, string secret)
    {
        return await _janusPluginService.HandlePluginResponse(
            new JanusAclDto
            {
                Request = "allowed",
                Action = "enable",
                Room = roomId,
                Secret = secret
            }
        );
    }
    
    private async Task<Result> AllowedRequest(string action, string token, string roomId, string secret)
    {
        return await _janusPluginService.HandlePluginResponse(
            new JanusAclDto
            {
                Request = "allowed",
                Action = action,
                Room = roomId,
                Secret = secret,
                Allowed = [token]
            }
        );
    }

    public async Task<Result> AllowToken(string token, string roomId, string secret)
    {
        return await AllowedRequest("add", token, roomId, secret);
    }

    public async Task<Result> DisallowToken(string token, string roomId, string secret)
    {
        return await AllowedRequest("remove", token, roomId, secret);
    }

    public async Task<Result> DestroyRoom(string roomId, string secret)
    {
        return await _janusPluginService.HandlePluginResponse(
            new JanusTextroomDto
            {
                Request = "destroy",
                Room = roomId,
                Secret = secret
            }
        );
    }
}