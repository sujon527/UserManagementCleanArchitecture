using MediatR;
using UserManagement.Application.DTOs.Responses;

namespace UserManagement.Application.DTOs.Requests;

public record ManageUserStatusRequest(Guid UserId, string Action, string Reason, string ActorId) : IRequest<Unit>;

public record AssignRoleRequest(Guid UserId, string Role, string ActorId) : IRequest<Unit>;

public record SearchUsersRequest(
    string? Query, 
    string? Role, 
    string? Status, 
    int Page = 1, 
    int PageSize = 10
) : IRequest<PagedResponse<UserProfileResponse>>;

public record PagedResponse<T>(IEnumerable<T> Items, long TotalCount, int Page, int PageSize);
