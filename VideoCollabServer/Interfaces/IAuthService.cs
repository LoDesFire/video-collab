using VideoCollabServer.Dtos;
using VideoCollabServer.Dtos.User;
using VideoCollabServer.Models;

namespace VideoCollabServer.Interfaces;

public interface IAuthService
{
    Task<Result<AuthedUserDto>> LoginAsync(AuthUserDto authUserDto);
    Task<Result<AuthedUserDto>> RegisterAsync(AuthUserDto authUserDto);
}