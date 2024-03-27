using Microsoft.AspNetCore.Identity;
using VideoCollabServer.Dtos.User;
using VideoCollabServer.Interfaces;
using VideoCollabServer.Models;

namespace VideoCollabServer.Services;

public class AuthService: IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;

    public AuthService(UserManager<User> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }
    
    public async Task<AuthResult> LoginAsync(AuthUserDto authUserDto)
    {
        var user = await _userManager.FindByNameAsync(authUserDto.UserName);
        if (user == null)
        {
            return new AuthResult
            {
                Errors = new List<string> { "Invalid username or password" }
            };
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, authUserDto.Password);
        if (isValidPassword)
        {
            return new AuthResult
            {
                Succeeded = true,
                Token = _tokenService.GenerateToken(authUserDto.UserName)
            };
        }

        return new AuthResult
        {
            Errors = new List<string> { "Invalid username or password" }
        };
    }

    public async Task<AuthResult> RegisterAsync(AuthUserDto authUserDto)
    {
        var user = new User{ UserName = authUserDto.UserName};
        var result = await _userManager.CreateAsync(user, authUserDto.Password);
        if (result.Succeeded)
        {
            return new AuthResult
            {
                Succeeded = true,
                Token = _tokenService.GenerateToken(authUserDto.UserName)
            };
        }

        return new AuthResult
        {
            Errors = result.Errors.Select(x => x.Description)
        };
        
    }
}