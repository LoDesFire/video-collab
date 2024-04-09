using VideoCollabServer.Dtos.User;
using VideoCollabServer.Models;

namespace VideoCollabServer.Interfaces;

public interface IUserRepository
{
    public Task<UserProfileDto?> GetByIdAsync(string id);

    public Task<AuthResult> CreateAsync(AuthUserDto authUserDto);
    public Task<AuthResult> LoginAsync(AuthUserDto authUserDto);

    public Task<bool> PinMovieAsync(string id, int movieId);
    public Task<bool> UnpinMovieAsync(string id, int movieId);
}