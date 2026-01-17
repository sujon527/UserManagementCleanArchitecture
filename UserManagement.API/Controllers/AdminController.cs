using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs.Requests;
using UserManagement.Application.Interfaces;

namespace UserManagement.API.Controllers;

public class AdminController : BaseApiController
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService) => _adminService = adminService;

    [HttpPost("{id}/status")]
    public async Task<IActionResult> ManageStatus(Guid id, ManageUserStatusRequest request)
    {
        if (id != request.UserId) return BadRequest();
        await _adminService.ManageStatusAsync(request with { ActorId = GetUserId().ToString() });
        return NoContent();
    }

    [HttpPost("{id}/roles")]
    public async Task<IActionResult> AssignRole(Guid id, AssignRoleRequest request)
    {
        if (id != request.UserId) return BadRequest();
        await _adminService.AssignRoleAsync(request with { ActorId = GetUserId().ToString() });
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] SearchUsersRequest request)
    {
        var result = await _adminService.SearchUsersAsync(request);
        return Ok(result);
    }
}
