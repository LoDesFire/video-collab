using VideoCollabServer.Dtos;
using VideoCollabServer.Interfaces;

namespace VideoCollabServer.Services;

public class JanusTextroomService(IJanusService janusService) : IJanusTextroomService
{
    private const string PluginName = "textroom";

    private string? _endpoint;

    private async Task<Result> ActivateAcl(string roomId)
    {
        if (_endpoint == null)
            return Result.Error("Obtain UrlPostfix first");

        var res = await janusService.SendMessage(_endpoint,
            new JanusAclDto
            {
                Textroom = "allowed",
                Action = "enable",
                Room = roomId,
                Secret = janusService.PluginSecrets[PluginName]
            }
        );

        if (!res.Succeeded)
            return Result.Error("Internal Server Error");

        var pluginData = res.Value.GetProperty("data");
        return pluginData.GetProperty("textroom").ToString() == "error"
            ? Result.Error(pluginData.GetProperty("error").ToString())
            : Result.Ok();
    }

    public async Task<Result> AllowToken(string token, string roomId)
    {
        if (_endpoint == null)
            return Result.Error("Obtain UrlPostfix first");

        var res = await janusService.SendMessage(_endpoint,
            new JanusAclDto
            {
                Textroom = "allowed",
                Action = "add",
                Room = roomId,
                Secret = janusService.PluginSecrets[PluginName],
                Allowed = [token]
            }
        );

        if (!res.Succeeded)
            return Result.Error("Internal Server Error");

        var pluginData = res.Value.GetProperty("data");
        return pluginData.GetProperty("textroom").ToString() == "error"
            ? Result.Error(pluginData.GetProperty("error").ToString())
            : Result.Ok();
    }

    public async Task<Result> ConnectToPlugin()
    {
        var res = await janusService.GetUrlPostfix(PluginName);
        if (!res.Succeeded)
            return Result.Error(res.Errors);

        _endpoint = res.Value;
        return Result.Ok();
    }

    public async Task<Result> CreateRoom(string roomId, string secret, bool @private)
    {
        if (_endpoint == null)
            return Result.Error("Obtain UrlPostfix first");

        var res = await janusService.SendMessage(_endpoint, new JanusCreateTextroomDto
        {
            AdminKey = janusService.PluginSecrets[PluginName],
            Request = "create",
            Secret = secret,
            Room = roomId,
            Private = @private
        });

        if (!res.Succeeded)
            return Result.Error(res.Errors);
        var pluginData = res.Value.GetProperty("data");
        if (pluginData.GetProperty("textroom").ToString() == "error")
            return Result.Error(pluginData.GetProperty("error").ToString());

        return await ActivateAcl(roomId);
    }
}