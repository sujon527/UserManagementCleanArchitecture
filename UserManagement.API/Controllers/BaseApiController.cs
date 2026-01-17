using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
    protected string GetUserRole() => User.FindFirstValue(ClaimTypes.Role) ?? "User";
}
