using Microsoft.AspNetCore.Identity;
using VideoCollab.Core.Domain;
using VideoCollab.Core.Domain.Abstractions;
using VideoCollab.Core.Domain.Models;

namespace VideoCollab.Core.Services;

public class AuthService(UserManager<User> userManager, ITokenService tokenService) : IAuthService
{
    public async Task<Result<string>> LoginAsync(string username, string password)
    {
        var user = await userManager.FindByNameAsync(username);
        if (user == null)
        {
            return new Result<string>
            {
                Errors = new List<string> { "Invalid username or password" }
            };
        }
        var userId = await userManager.GetUserIdAsync(user);

        var isValidPassword = await userManager.CheckPasswordAsync(user, password);
        if (isValidPassword)
        {
            return new Result<string>
            {
                Succeeded = true,
                Value = tokenService.GenerateToken(userId)
            };
        }

        return new Result<string>
        {
            Errors = new List<string> { "Invalid username or password" }
        };
    }

    public async Task<Result<string>> RegisterAsync(string username, string password)
    {
        var user = new User { UserName = username };
        var result = await userManager.CreateAsync(user, password);

        var userId = await userManager.GetUserIdAsync(user);
        
        if (result.Succeeded)
        {
            return new Result<string>
            {
                Succeeded = true,
                Value = tokenService.GenerateToken(userId)
            };
        }

        return new Result<string>
        {
            Errors = result.Errors.Select(x => x.Description)
        };
    }
}