using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs.Requests;

namespace UserManagement.API.Controllers;

public class UsersController : BaseApiController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        var id = await _mediator.Send(request);
        return CreatedAtAction(nameof(GetProfile), new { id }, new { id });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _mediator.Send(new GetUserProfileRequest(GetUserId(), GetUserId(), GetUserRole()));
        return Ok(result);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile(UpdateUserProfileRequest request)
    {
        if (request.UserId != GetUserId()) return BadRequest("UserID mismatch.");
        await _mediator.Send(request);
        return NoContent();
    }

    [HttpPost("me/change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        if (request.UserId != GetUserId()) return BadRequest("UserID mismatch.");
        await _mediator.Send(request);
        return NoContent();
    }
}
