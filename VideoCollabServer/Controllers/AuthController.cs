using Microsoft.AspNetCore.Mvc;
using VideoCollabServer.Dtos.User;
using VideoCollabServer.Interfaces;
using VideoCollabServer.Utils;

namespace VideoCollabServer.Controllers;

[Route("api")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _repository;
    
    public AuthController(IUserRepository repository)
    {
        _repository = repository;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthUserDto authUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetErrorsList());

        var authResult = await _repository.LoginAsync(authUserDto);

        return authResult.Succeeded ? Ok(authResult.Value) : StatusCode(401, authResult.Errors);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthUserDto authUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetErrorsList());

        var authResult = await _repository.CreateAsync(authUserDto);

        return authResult.Succeeded ? Ok(authResult.Value) : StatusCode(401, authResult.Errors);
    }
}