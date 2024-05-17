using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoCollabServer.Interfaces;

namespace VideoCollabServer.Controllers;

[Route("api/room")]
[ApiController]
public class RoomController(IRoomRepository roomRepository, IJanusService janusService): ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateRoom([FromQuery] bool @private)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var id = identity!.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")!.Value;
        
        var r= await roomRepository.CreateRoomAsync(id, @private);
        if (!r.Succeeded)
            return BadRequest(r.Errors);
        
        return Ok(r.Value);
    }
    
    [HttpGet("join")]
    public async Task<IActionResult> AllowToken([FromQuery] string roomId)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var id = identity!.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")!.Value;
        
        var r= await roomRepository.JoinTheRoomAsync(id, roomId);
        if (!r.Succeeded)
            return BadRequest(r.Errors);
        
        return Ok();
    }
}