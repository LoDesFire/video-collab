using VideoCollabServer.Dtos;
using VideoCollabServer.Interfaces;

namespace VideoCollabServer.Services;

public class JanusPluginService(IConfiguration configuration) : IJanusPluginService
{
    private string? _endpoint;
    private string _pluginName = "";
    private readonly JanusService _janusService = new (configuration);

    public string Secret { get; private set; } = "";
    
    public string PluginName
    {
        get => _pluginName;
        set
        {
            _pluginName = value;
            Secret = _janusService.PluginSecrets[PluginName];
        }
    }

    public async Task<Result> HandlePluginResponse(object message)
    {
        if (_endpoint == null)
        {
            var connectionRes = await ConnectToPlugin();
            if (!connectionRes.Succeeded)
                return Result.Error(connectionRes.Errors);
        }

        var res = await _janusService.SendMessage(_endpoint!, message);
        if (!res.Succeeded)
            return Result.Error("Internal Server Error");

        var pluginData = res.Value.GetProperty("data");
        return pluginData.GetProperty(PluginName).ToString() == "error"
            ? Result.Error(pluginData.GetProperty("error").ToString())
            : Result.Ok();
    }
    
    private async Task<Result> ConnectToPlugin()
    {
        var res = await _janusService.GetUrlPostfix(PluginName);
        if (!res.Succeeded)
            return Result.Error(res.Errors);

        _endpoint = res.Value;
        return Result.Ok();
    }
}