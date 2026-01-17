using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs.Requests;
using UserManagement.Application.Interfaces;

namespace UserManagement.API.Controllers;

public class UsersController : BaseApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        var id = await _userService.RegisterAsync(request);
        return CreatedAtAction(nameof(GetProfile), new { id }, new { id });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _userService.GetProfileAsync(new GetUserProfileRequest(GetUserId(), GetUserId(), GetUserRole()));
        return Ok(result);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile(UpdateUserProfileRequest request)
    {
        if (request.UserId != GetUserId()) return BadRequest("UserID mismatch.");
        await _userService.UpdateProfileAsync(request);
        return NoContent();
    }

    [HttpPost("me/change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        if (request.UserId != GetUserId()) return BadRequest("UserID mismatch.");
        await _userService.ChangePasswordAsync(request);
        return NoContent();
    }
}
