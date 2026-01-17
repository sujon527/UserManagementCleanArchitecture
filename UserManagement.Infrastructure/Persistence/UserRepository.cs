using MongoDB.Driver;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;
using UserManagement.Domain.ValueObjects;

namespace UserManagement.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly MongoDbContext _context;

    public UserRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id) => 
        await _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync();

    public async Task<User?> GetByUsernameAsync(string username) => 
        await _context.Users.Find(u => u.Username == username).FirstOrDefaultAsync();

    public async Task<User?> GetByEmailAsync(Email email) => 
        await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();

    public async Task<User?> GetByPhoneAsync(PhoneNumber phone) => 
        await _context.Users.Find(u => u.Phone == phone).FirstOrDefaultAsync();

    public async Task AddAsync(User user) => 
        await _context.Users.InsertOneAsync(user);

    public async Task UpdateAsync(User user)
    {
        var result = await _context.Users.ReplaceOneAsync(u => u.Id == user.Id && u.Version == user.Version - 1, user);
        if (result.ModifiedCount == 0)
            throw new Exception("Concurrency exception: Update failed.");
    }

    public async Task<IEnumerable<User>> SearchAsync(string? query, string? role, string? status, int page, int pageSize)
    {
        var filter = BuildFilter(query, role, status);
        return await _context.Users.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<long> CountAsync(string? query, string? role, string? status)
    {
        var filter = BuildFilter(query, role, status);
        return await _context.Users.CountDocumentsAsync(filter);
    }

    private FilterDefinition<User> BuildFilter(string? query, string? role, string? status)
    {
        var builder = Builders<User>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrWhiteSpace(query))
            filter &= builder.Or(builder.Regex(u => u.FullName, new MongoDB.Bson.BsonRegularExpression(query, "i")), builder.Regex(u => u.Username, new MongoDB.Bson.BsonRegularExpression(query, "i")));

        if (!string.IsNullOrWhiteSpace(role))
            filter &= builder.AnyEq(u => u.Roles, role);

        if (!string.IsNullOrWhiteSpace(status))
            filter &= builder.Eq(u => u.Status.ToString(), status);

        return filter;
    }
}
