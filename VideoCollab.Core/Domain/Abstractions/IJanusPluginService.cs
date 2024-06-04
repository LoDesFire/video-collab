namespace VideoCollab.Core.Domain.Abstractions;

public interface IJanusPluginService
{ 
    string PluginName { set; }
    string Secret { get; }
    Task<Result> HandlePluginResponse(object message);
}