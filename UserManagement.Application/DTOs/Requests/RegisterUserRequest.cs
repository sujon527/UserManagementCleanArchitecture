using MediatR;

namespace UserManagement.Application.DTOs.Requests;

public record RegisterUserRequest(
    string Username,
    string Email,
    string Phone,
    string FullName,
    string Password,
    bool MarketingConsent,
    int TermsVersion
) : IRequest<Guid>;
