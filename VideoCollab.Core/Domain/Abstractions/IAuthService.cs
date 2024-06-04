namespace VideoCollab.Core.Domain.Abstractions;

public interface IAuthService
{
    Task<Result<string>> LoginAsync(string username, string password);
    Task<Result<string>> RegisterAsync(string username, string password);
}