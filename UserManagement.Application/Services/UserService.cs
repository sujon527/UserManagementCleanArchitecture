using UserManagement.Application.DTOs.Requests;
using UserManagement.Application.DTOs.Responses;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;
using UserManagement.Domain.ValueObjects;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IAuditRepository auditRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _auditRepository = auditRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> RegisterAsync(RegisterUserRequest request)
    {
        var email = new Email(request.Email);
        var phone = new PhoneNumber(request.Phone);

        if (await _userRepository.GetByEmailAsync(email) != null)
            throw new UserDomainException("Email already exists.");
        
        if (await _userRepository.GetByPhoneAsync(phone) != null)
            throw new UserDomainException("Phone number already exists.");

        if (await _userRepository.GetByUsernameAsync(request.Username) != null)
            throw new UserDomainException("Username already exists.");

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = new User(
            request.Username,
            email,
            phone,
            request.FullName,
            passwordHash,
            request.TermsVersion,
            request.MarketingConsent
        );

        await _userRepository.AddAsync(user);

        var audit = new AuditLog(user.Id, "Register", "User registered", "N/A", "N/A", user.Id.ToString());
        await _auditRepository.AddAsync(audit);

        return user.Id;
    }

    public async Task<UserProfileResponse> GetProfileAsync(GetUserProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId) 
            ?? throw new UserDomainException("User not found.");

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

    public async Task UpdateProfileAsync(UpdateUserProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId) 
            ?? throw new UserDomainException("User not found.");

        if (user.Version != request.Version)
            throw new UserDomainException("Concurrency conflict: Profile has been updated by another process.");

        user.UpdateProfile(request.FullName, new PhoneNumber(request.Phone));
        await _userRepository.UpdateAsync(user);

        await _auditRepository.AddAsync(new AuditLog(user.Id, "UpdateProfile", "Profile updated", "N/A", "N/A", request.UserId.ToString()));
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId) 
            ?? throw new UserDomainException("User not found.");

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new UserDomainException("Incorrect current password.");

        var newHash = _passwordHasher.Hash(request.NewPassword);
        user.ChangePassword(newHash);
        await _userRepository.UpdateAsync(user);

        await _auditRepository.AddAsync(new AuditLog(user.Id, "ChangePassword", "Password changed", "N/A", "N/A", request.UserId.ToString()));
    }

    private static string MaskPhone(string phone) 
    {
        if (phone.Length < 7) return "****";
        return phone.Substring(0, 4) + "****" + phone.Substring(phone.Length - 3);
    }
}
