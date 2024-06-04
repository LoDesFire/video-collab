using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RestSharp;
using VideoCollab.Core.Domain;
using VideoCollab.Core.Domain.DTOs;

namespace VideoCollab.Core.Services;

public class JanusService
{
    public string AdminSecret { get; set; }
    private string BaseUrl { get; }
    public Dictionary<string, string> PluginSecrets { get; } = new();

    private readonly RestClient _client;

    public JanusService(IConfiguration configuration)
    {
        AdminSecret = configuration.GetSection("Janus")["adminSecret"]!;
        BaseUrl = configuration.GetSection("Janus")["url"]!;
        var secretsSections = configuration.GetSection("Janus").GetSection("plugins").GetChildren();

        foreach (var section in secretsSections)
            PluginSecrets.Add(section.Key, section.Value!);

        var options = new RestClientOptions(BaseUrl);
        _client = new RestClient(options);
    }
    
    private async Task<Result<ulong>> GetSessionId()
    {
        var request = new RestRequest("", Method.Post);
        request.AddJsonBody(new JanusDto { JanusAction = "create", Transaction = Guid.NewGuid().ToString() });

        var response = await _client.ExecuteAsync<JanusResponseDto>(request);
        if (!response.IsSuccessStatusCode)
            return Result<ulong>.Error(response.ErrorMessage!);
        if (response.Data!.JanusAction == "error")
            return ParseError<ulong>(response.Data!.Error);
        
        var sessionId = response.Data?.Data?.GetProperty("id").GetUInt64();
        return sessionId.HasValue
            ? Result<ulong>.Ok(sessionId.Value)
            : Result<ulong>.Error("Unable to obtain janus session ID");
    }
    
    private async Task<Result<ulong>> GetHandleId(ulong sessionId, string pluginName)
    {
        if (!PluginSecrets.TryGetValue(pluginName, out _))
            return Result<ulong>.Error("Unknown plugin name");
        
        var request = new RestRequest(sessionId.ToString(), Method.Post);
        request.AddJsonBody(
            new JanusAttachDto
            {
                JanusAction = "attach",
                Transaction = Guid.NewGuid().ToString(),
                Plugin = $"janus.plugin.{pluginName}"
            }
        );

        var response = await _client.ExecuteAsync<JanusResponseDto>(request);
        if (!response.IsSuccessStatusCode)
            return Result<ulong>.Error(response.ErrorMessage!);
        if (response.Data!.JanusAction == "error")
            return ParseError<ulong>(response.Data!.Error);

        var handleId = response.Data?.Data?.GetProperty("id").GetUInt64();
        return handleId.HasValue
            ? Result<ulong>.Ok(handleId.Value)
            : Result<ulong>.Error("Unable to obtain janus session ID");
    }

    private static Result<T> ParseError<T>(JsonElement? janusResponse)
    {
        var error = janusResponse?.GetProperty("error").ToString();
        return error != null ? Result<T>.Error(error) : Result<T>.Error("Unable to obtain the error");
    }

    public async Task<Result<string>> GetUrlPostfix(string pluginName)
    {
        var sId = await GetSessionId();
        if (!sId.Succeeded)
            return Result<string>.Error(sId.Errors);
        
        var hId = await GetHandleId(sId.Value, pluginName);
        return !hId.Succeeded ? Result<string>.Error(hId.Errors) : Result<string>.Ok($"{sId.Value}/{hId.Value}");
    }

    public async  Task<Result<JsonElement>> SendMessage(string endpoint, object message)
    {
        var request = new RestRequest(endpoint, Method.Post);
        request.AddJsonBody(
            new JanusMessageDto
            {
                JanusAction = "message",
                Transaction = Guid.NewGuid().ToString(),
                Body = message
            }
        );
        
        var response = await _client.ExecuteAsync<JanusResponseDto>(request);
        if (!response.IsSuccessStatusCode)
            return Result<JsonElement>.Error(response.ErrorMessage!);
        if (response.Data!.JanusAction == "error")
            return ParseError<JsonElement>(response.Data!.Error);
        
        var data = response.Data?.Plugindata;
        return data.HasValue
            ? Result<JsonElement>.Ok(data.Value)
            : Result<JsonElement>.Error("Unknown response from Janus");
    }
}