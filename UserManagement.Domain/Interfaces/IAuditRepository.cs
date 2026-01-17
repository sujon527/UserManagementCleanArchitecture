using UserManagement.Domain.Entities;

namespace UserManagement.Domain.Interfaces;

public interface IAuditRepository
{
    Task AddAsync(AuditLog log);
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId);
}
