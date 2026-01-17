using MediatR;
using UserManagement.Application.DTOs.Responses;

namespace UserManagement.Application.DTOs.Requests;

public record GetUserProfileRequest(Guid UserId, Guid RequesterId, string RequesterRole) : IRequest<UserProfileResponse>;

public record UpdateUserProfileRequest(
    Guid UserId, 
    string FullName, 
    string Phone, 
    long Version
) : IRequest<Unit>;

public record ChangePasswordRequest(
    Guid UserId, 
    string CurrentPassword, 
    string NewPassword
) : IRequest<Unit>;
