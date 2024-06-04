using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoCollab.Core.Domain.Abstractions;
using VideoCollabServer.Dtos.Room;
using VideoCollabServer.Dtos.User;

namespace VideoCollabServer.Controllers;

[Route("api/room")]
[ApiController]
public class RoomController(IRoomService roomService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateRoom([FromQuery] bool @private)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var id = identity!.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")!.Value;

        var r = await roomService.CreateRoomAsync(id, @private);
        if (!r.Succeeded)
            return BadRequest(r.Errors);

        var (roomId, (userId, username, textRoomToken)) = r.Value;

        return Ok(
            new CreatedRoomDto
            {
                Id = roomId,
                Owner = new JoinedUserDto
                {
                    Id = userId,
                    TextroomToken = textRoomToken,
                    Username = username
                },
            }
        );
    }

    [HttpGet("join")]
    [Authorize]
    public async Task<IActionResult> AllowToken([FromQuery] string roomId)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var id = identity!.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")!.Value;

        var r = await roomService.JoinTheRoomAsync(id, roomId);
        if (!r.Succeeded)
            return BadRequest(r.Errors);

        var (userId, username, textRoomToken) = r.Value;

        return Ok(
            new JoinedUserDto
            {
                Id = userId,
                TextroomToken = textRoomToken,
                Username = username
            }
        );
    }

    [HttpGet("leave")]
    [Authorize]
    public async Task<IActionResult> LeaveTheRoom([FromQuery] string roomId)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var id = identity!.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")!.Value;

        await roomService.LeaveFromRoom(id, roomId);

        return Ok();
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DestroyRoom([FromQuery] string roomId)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var id = identity!.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")!.Value;

        await roomService.DeleteRoomAsync(id, roomId);

        return Ok();
    }
}