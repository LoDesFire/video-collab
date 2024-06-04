namespace VideoCollab.Core.Domain.Abstractions;

public interface ITokenService
{ 
    string GenerateToken(string username);
}