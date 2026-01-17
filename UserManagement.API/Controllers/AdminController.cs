using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserManagement.Application.DTOs.Requests;

namespace UserManagement.API.Controllers;

// [Authorize(Roles = "Admin")] // Uncomment when JWT is added
public class AdminController : BaseApiController
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator) => _mediator = mediator;

    [HttpPost("{id}/status")]
    public async Task<IActionResult> ManageStatus(Guid id, ManageUserStatusRequest request)
    {
        if (id != request.UserId) return BadRequest();
        await _mediator.Send(request with { ActorId = GetUserId().ToString() });
        return NoContent();
    }

    [HttpPost("{id}/roles")]
    public async Task<IActionResult> AssignRole(Guid id, AssignRoleRequest request)
    {
        if (id != request.UserId) return BadRequest();
        await _mediator.Send(request with { ActorId = GetUserId().ToString() });
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] SearchUsersRequest request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }
}
