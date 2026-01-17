using MediatR;
using UserManagement.Application.DTOs.Requests;
using UserManagement.Application.DTOs.Responses;
using UserManagement.Domain.Interfaces;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Application.UseCases.Handlers;

public class AdminHandlers : 
    IRequestHandler<ManageUserStatusRequest, Unit>,
    IRequestHandler<AssignRoleRequest, Unit>,
    IRequestHandler<SearchUsersRequest, PagedResponse<UserProfileResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditRepository _auditRepository;

    public AdminHandlers(IUserRepository userRepository, IAuditRepository auditRepository)
    {
        _userRepository = userRepository;
        _auditRepository = auditRepository;
    }

    public async Task<Unit> Handle(ManageUserStatusRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId) 
            ?? throw new UserDomainException("User not found.");

        if (request.Action == "Deactivate") 
            user.Deactivate(request.Reason);
        else 
            user.Activate();

        await _userRepository.UpdateAsync(user);
        await _auditRepository.AddAsync(new Domain.Entities.AuditLog(user.Id, request.Action, request.Reason, "N/A", "N/A", request.ActorId));
        return Unit.Value;
    }

    public async Task<Unit> Handle(AssignRoleRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId) 
            ?? throw new UserDomainException("User not found.");

        user.AddRole(request.Role);
        await _userRepository.UpdateAsync(user);
        await _auditRepository.AddAsync(new Domain.Entities.AuditLog(user.Id, "AssignRole", $"Assigned role: {request.Role}", "N/A", "N/A", request.ActorId));
        return Unit.Value;
    }

    public async Task<PagedResponse<UserProfileResponse>> Handle(SearchUsersRequest request, CancellationToken cancellationToken)
    {
        var items = await _userRepository.SearchAsync(request.Query, request.Role, request.Status, request.Page, request.PageSize);
        var total = await _userRepository.CountAsync(request.Query, request.Role, request.Status);

        var responses = items.Select(u => new UserProfileResponse(
            u.Id, u.Username, u.Email.Value, u.Phone.Value, u.FullName, u.Status.ToString(), u.Roles, u.CreatedAt
        ));

        return new PagedResponse<UserProfileResponse>(responses, total, request.Page, request.PageSize);
    }
}
