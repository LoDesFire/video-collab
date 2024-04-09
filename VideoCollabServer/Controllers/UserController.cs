using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserProfile([FromRoute] string id)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (identity!.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value != id)
            return Forbid();
        
        var profileDto = await _repository.GetByIdAsync(id);
        return Ok(profileDto);
    }
}