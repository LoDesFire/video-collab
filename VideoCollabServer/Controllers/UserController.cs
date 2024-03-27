using Microsoft.AspNetCore.Mvc;
using VideoCollabServer.Dtos.User;
using VideoCollabServer.Interfaces;

namespace VideoCollabServer.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _repository;

    public UserController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthUserDto authUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var authResult = await _repository.LoginAsync(authUserDto);
        
        return authResult.Succeeded ? Ok(authResult.Token) : StatusCode(401, authResult.Errors);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthUserDto authUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var authResult = await _repository.CreateAsync(authUserDto);
        
        return authResult.Succeeded ? Ok(authResult.Token) : StatusCode(400, authResult.Errors);
    }
    
    
}