using UserManagement.Domain.Entities;
using UserManagement.Domain.ValueObjects;

namespace UserManagement.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(Email email);
    Task<User?> GetByPhoneAsync(PhoneNumber phone);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<IEnumerable<User>> SearchAsync(string? query, string? role, string? status, int page, int pageSize);
    Task<long> CountAsync(string? query, string? role, string? status);
}
