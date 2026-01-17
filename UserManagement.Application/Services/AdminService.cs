using UserManagement.Application.DTOs.Requests;
using UserManagement.Application.DTOs.Responses;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Application.Services;

public class AdminService : IAdminService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditRepository _auditRepository;

    public AdminService(IUserRepository userRepository, IAuditRepository auditRepository)
    {
        _userRepository = userRepository;
        _auditRepository = auditRepository;
    }

    public async Task ManageStatusAsync(ManageUserStatusRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId) 
            ?? throw new UserDomainException("User not found.");

        if (request.Action == "Deactivate") 
            user.Deactivate(request.Reason);
        else 
            user.Activate();

        await _userRepository.UpdateAsync(user);
        await _auditRepository.AddAsync(new AuditLog(user.Id, request.Action, request.Reason, "N/A", "N/A", request.ActorId));
    }

    public async Task AssignRoleAsync(AssignRoleRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId) 
            ?? throw new UserDomainException("User not found.");

        user.AddRole(request.Role);
        await _userRepository.UpdateAsync(user);
        await _auditRepository.AddAsync(new AuditLog(user.Id, "AssignRole", $"Assigned role: {request.Role}", "N/A", "N/A", request.ActorId));
    }

    public async Task<PagedResponse<UserProfileResponse>> SearchUsersAsync(SearchUsersRequest request)
    {
        var items = await _userRepository.SearchAsync(request.Query, request.Role, request.Status, request.Page, request.PageSize);
        var total = await _userRepository.CountAsync(request.Query, request.Role, request.Status);

        var responses = items.Select(u => new UserProfileResponse(
            u.Id, u.Username, u.Email.Value, u.Phone.Value, u.FullName, u.Status.ToString(), u.Roles, u.CreatedAt
        ));

        return new PagedResponse<UserProfileResponse>(responses, total, request.Page, request.PageSize);
    }
}
