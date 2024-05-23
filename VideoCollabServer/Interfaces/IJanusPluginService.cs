using VideoCollabServer.Dtos;

namespace VideoCollabServer.Interfaces;

public interface IJanusPluginService
{ 
    string PluginName { set; }
    string Secret { get; }
    Task<Result> HandlePluginResponse(object message);
}