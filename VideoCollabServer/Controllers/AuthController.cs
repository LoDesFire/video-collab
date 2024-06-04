using Microsoft.AspNetCore.Mvc;
using VideoCollab.Core.Domain.Abstractions;
using VideoCollabServer.Dtos.User;
using VideoCollabServer.Utils;

namespace VideoCollabServer.Controllers;

[Route("api")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthUserDto authUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetErrorsList());

        var token = await authService.LoginAsync(authUserDto.Username, authUserDto.Password);

        return token.Succeeded
            ? Ok(
                new AuthedUserDto
                {
                    Token = token.Value!,
                    Username = authUserDto.Username
                }
            )
            : StatusCode(401, token.Errors);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthUserDto authUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetErrorsList());

        var authResult = await authService.RegisterAsync(authUserDto.Username, authUserDto.Password);

        return authResult.Succeeded
            ? Ok(
                new AuthedUserDto
                {
                    Token = authResult.Value!,
                    Username = authUserDto.Username
                }
            )
            : StatusCode(401, authResult.Errors);
    }
}