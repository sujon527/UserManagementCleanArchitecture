using UserManagement.Application.DTOs.Requests;
using UserManagement.Application.DTOs.Responses;

namespace UserManagement.Application.Interfaces;

public interface IUserService
{
    Task<Guid> RegisterAsync(RegisterUserRequest request);
    Task<UserProfileResponse> GetProfileAsync(GetUserProfileRequest request);
    Task UpdateProfileAsync(UpdateUserProfileRequest request);
    Task ChangePasswordAsync(ChangePasswordRequest request);
}
