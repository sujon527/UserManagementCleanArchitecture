using UserManagement.Application.DTOs.Requests;
using UserManagement.Application.DTOs.Responses;

namespace UserManagement.Application.Interfaces;

public interface IAdminService
{
    Task ManageStatusAsync(ManageUserStatusRequest request);
    Task AssignRoleAsync(AssignRoleRequest request);
    Task<PagedResponse<UserProfileResponse>> SearchUsersAsync(SearchUsersRequest request);
}
