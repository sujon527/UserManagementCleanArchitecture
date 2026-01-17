using MediatR;
using UserManagement.Application.DTOs.Requests;
using UserManagement.Application.DTOs.Responses;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Interfaces;
using UserManagement.Domain.ValueObjects;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Application.UseCases.Handlers;

public class ProfileHandlers : 
    IRequestHandler<GetUserProfileRequest, UserProfileResponse>,
    IRequestHandler<UpdateUserProfileRequest, Unit>,
    IRequestHandler<ChangePasswordRequest, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ProfileHandlers(IUserRepository userRepository, IAuditRepository auditRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _auditRepository = auditRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserProfileResponse> Handle(GetUserProfileRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId) 
            ?? throw new UserDomainException("User not found.");

        // Authorization: Self or Admin
        if (request.RequesterId != request.UserId && request.RequesterRole != "Admin")
            throw new UserDomainException("Unauthorized access to profile.");

        return new UserProfileResponse(
            user.Id,
            user.Username,
            user.Email.Value,
            MaskPhone(user.Phone.Value),
            user.FullName,
            user.Status.ToString(),
            user.Roles,
            user.CreatedAt
        );
    }

    public async Task<Unit> Handle(UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId) 
            ?? throw new UserDomainException("User not found.");

        if (user.Version != request.Version)
            throw new UserDomainException("Concurrency conflict: Profile has been updated by another process.");

        user.UpdateProfile(request.FullName, new PhoneNumber(request.Phone));
        await _userRepository.UpdateAsync(user);

        await _auditRepository.AddAsync(new Domain.Entities.AuditLog(user.Id, "UpdateProfile", "Profile updated", "N/A", "N/A", request.UserId.ToString()));
        return Unit.Value;
    }

    public async Task<Unit> Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId) 
            ?? throw new UserDomainException("User not found.");

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new UserDomainException("Incorrect current password.");

        var newHash = _passwordHasher.Hash(request.NewPassword);
        
        // Rule: Password history check (conceptual) 
        // In real app, check user.PasswordHistory list

        user.ChangePassword(newHash);
        await _userRepository.UpdateAsync(user);

        await _auditRepository.AddAsync(new Domain.Entities.AuditLog(user.Id, "ChangePassword", "Password changed", "N/A", "N/A", request.UserId.ToString()));
        return Unit.Value;
    }

    private static string MaskPhone(string phone) 
    {
        if (phone.Length < 7) return "****";
        return phone.Substring(0, 4) + "****" + phone.Substring(phone.Length - 3);
    }
}
