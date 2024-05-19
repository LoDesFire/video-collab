using Microsoft.AspNetCore.Mvc;
using VideoCollabServer.Dtos.User;
using VideoCollabServer.Interfaces;
using VideoCollabServer.Utils;

namespace VideoCollabServer.Controllers;

[Route("api")]
[ApiController]
public class AuthController(IUserRepository repository) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthUserDto authUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetErrorsList());

        var authResult = await repository.LoginAsync(authUserDto);

        return authResult.Succeeded ? Ok(authResult.Value) : StatusCode(401, authResult.Errors);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthUserDto authUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetErrorsList());

        var authResult = await repository.CreateAsync(authUserDto);

        return authResult.Succeeded ? Ok(authResult.Value) : StatusCode(401, authResult.Errors);
    }
}