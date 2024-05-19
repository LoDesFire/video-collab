using VideoCollabServer.Dtos;
using VideoCollabServer.Interfaces;

namespace VideoCollabServer.Services;

public class JanusTextroomService(IJanusService janusService) : IJanusTextroomService
{
    private const string PluginName = "textroom";

    private string? _endpoint;

    private async Task<Result> HandlePluginResponse(object message)
    {
        if (_endpoint == null)
        {
            var connectionRes = await ConnectToPlugin();
            if (!connectionRes.Succeeded)
                return Result.Error(connectionRes.Errors);
        }

        var res = await janusService.SendMessage(_endpoint!, message);
        if (!res.Succeeded)
            return Result.Error("Internal Server Error");

        var pluginData = res.Value.GetProperty("data");
        return pluginData.GetProperty("textroom").ToString() == "error"
            ? Result.Error(pluginData.GetProperty("error").ToString())
            : Result.Ok();
    }
    
    private async Task<Result> ConnectToPlugin()
    {
        var res = await janusService.GetUrlPostfix(PluginName);
        if (!res.Succeeded)
            return Result.Error(res.Errors);

        _endpoint = res.Value;
        return Result.Ok();
    }

    public async Task<Result> CreateRoom(string roomId, string secret, bool @private)
    {
        var res = await HandlePluginResponse(new JanusCreateTextroomDtoDto
        {
            AdminKey = janusService.PluginSecrets[PluginName],
            Request = "create",
            Secret = secret,
            Room = roomId,
            Private = @private
        });
        if (!res.Succeeded)
            return Result.Error(res.Errors);

        return await ActivateAcl(roomId, secret);
    }
    
    private async Task<Result> ActivateAcl(string roomId, string secret)
    {
        return await HandlePluginResponse(
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
        return await HandlePluginResponse(
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
        return await HandlePluginResponse(
            new JanusTextroomDto
            {
                Request = "destroy",
                Room = roomId,
                Secret = secret
            }
        );
    }
}