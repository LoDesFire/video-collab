using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoCollabServer.Interfaces;

namespace VideoCollabServer.Controllers;

[Route("api/user")]
[ApiController]
public class UserController(IUserRepository repository) : ControllerBase
{
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetUserProfile()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var id = identity!.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")!.Value;
        
        var profileDto = await repository.GetByIdAsync(id);
        return Ok(profileDto);
    }
    
    [Authorize]
    [HttpPut("pinnedMovies")]
    public async Task<IActionResult> PinMovie([FromQuery][Required] int movieId)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var id = identity!.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")!.Value;

        var result = await repository.PinMovieAsync(id, movieId);
        return result ? Ok() : BadRequest(new List<string> {"This movie doesn't exist"});
    }
    
    [Authorize]
    [HttpDelete("pinnedMovies")]
    public async Task<IActionResult> UnpinMovie([FromQuery][Required] int movieId)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var id = identity!.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")!.Value;

        var result = await repository.UnpinMovieAsync(id, movieId);
        return result ? Ok() : BadRequest(new List<string> {"This movie is already unpinned"});
    }
}