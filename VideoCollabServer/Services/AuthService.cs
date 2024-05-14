using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using VideoCollabServer.Dtos;
using VideoCollabServer.Dtos.User;
using VideoCollabServer.Interfaces;
using VideoCollabServer.Models;

namespace VideoCollabServer.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;

    public AuthService(UserManager<User> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthedUserDto>> LoginAsync(AuthUserDto authUserDto)
    {
        var user = await _userManager.FindByNameAsync(authUserDto.Username);
        if (user == null)
        {
            return new Result<AuthedUserDto>
            {
                Errors = new List<string> { "Invalid username or password" }
            };
        }
        var userId = await _userManager.GetUserIdAsync(user);

        var isValidPassword = await _userManager.CheckPasswordAsync(user, authUserDto.Password);
        if (isValidPassword)
        {
            return new Result<AuthedUserDto>
            {
                Succeeded = true,
                Value = new AuthedUserDto
                {
                    Id = userId, 
                    Token = _tokenService.GenerateToken(userId),
                    Username = authUserDto.Username
                }
            };
        }

        return new Result<AuthedUserDto>
        {
            Errors = new List<string> { "Invalid username or password" }
        };
    }

    public async Task<Result<AuthedUserDto>> RegisterAsync(AuthUserDto authUserDto)
    {
        var user = new User { UserName = authUserDto.Username };
        var result = await _userManager.CreateAsync(user, authUserDto.Password);

        var userId = await _userManager.GetUserIdAsync(user);
        
        if (result.Succeeded)
        {
            return new Result<AuthedUserDto>
            {
                Succeeded = true,
                Value = new AuthedUserDto
                {
                    Id = userId, 
                    Token = _tokenService.GenerateToken(userId),
                    Username = authUserDto.Username
                }
            };
        }

        return new Result<AuthedUserDto>
        {
            Errors = result.Errors.Select(x => x.Description)
        };
    }
}