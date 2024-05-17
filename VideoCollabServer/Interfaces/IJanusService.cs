using System.Text.Json;
using VideoCollabServer.Dtos;

namespace VideoCollabServer.Interfaces;

public interface IJanusService
{
    public Dictionary<string, string> PluginSecrets { get; }
    
    Task<Result<string>> GetUrlPostfix(string pluginName);
    
    Task<Result<JsonElement>> SendMessage(string endpoint, object message);
}