using MediatR;
using UserManagement.Application.DTOs.Requests;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;
using UserManagement.Domain.ValueObjects;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Application.UseCases.Handlers;

public class RegisterUserHandler : IRequestHandler<RegisterUserRequest, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserHandler(IUserRepository userRepository, IAuditRepository auditRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _auditRepository = auditRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);
        var phone = new PhoneNumber(request.Phone);

        // Business Rule: Identity uniqueness
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
}
