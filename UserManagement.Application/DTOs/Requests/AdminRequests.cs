using UserManagement.Application.DTOs.Responses;

namespace UserManagement.Application.DTOs.Requests;

public record ManageUserStatusRequest(Guid UserId, string Action, string Reason, string ActorId);

public record AssignRoleRequest(Guid UserId, string Role, string ActorId);

public record SearchUsersRequest(
    string? Query, 
    string? Role, 
    string? Status, 
    int Page = 1, 
    int PageSize = 10
);

public record PagedResponse<T>(IEnumerable<T> Items, long TotalCount, int Page, int PageSize);
