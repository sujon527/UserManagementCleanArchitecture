namespace UserManagement.Application.DTOs.Responses;

public record UserProfileResponse(
    Guid Id,
    string Username,
    string Email,
    string Phone,
    string FullName,
    string Status,
    List<string> Roles,
    DateTime CreatedAt
);
