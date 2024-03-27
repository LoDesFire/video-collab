using VideoCollabServer.Dtos.User;
using VideoCollabServer.Models;

namespace VideoCollabServer.Interfaces;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(AuthUserDto authUserDto);
    Task<AuthResult> RegisterAsync(AuthUserDto authUserDto);
}