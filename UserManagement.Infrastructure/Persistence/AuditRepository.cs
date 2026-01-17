using MongoDB.Driver;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Infrastructure.Persistence;

public class AuditRepository : IAuditRepository
{
    private readonly MongoDbContext _context;

    public AuditRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditLog log) => 
        await _context.AuditLogs.InsertOneAsync(log);

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId) => 
        await _context.AuditLogs.Find(l => l.UserId == userId).ToListAsync();
}
