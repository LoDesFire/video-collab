namespace VideoCollabServer.Interfaces;

public interface ITokenService
{ 
    string GenerateToken(string username);
}