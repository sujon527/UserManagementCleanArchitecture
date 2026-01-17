using MongoDB.Driver;
using UserManagement.Domain.Entities;
using UserManagement.Domain.ValueObjects;
using MongoDB.Bson.Serialization;

namespace UserManagement.Infrastructure.Persistence;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
        
        ConfigureMappings();
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    public IMongoCollection<AuditLog> AuditLogs => _database.GetCollection<AuditLog>("AuditLogs");

    private static void ConfigureMappings()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(User))) return;

        BsonClassMap.RegisterClassMap<User>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(c => c.Id);
            cm.MapField("Username").SetElementName("Username");
            cm.MapField("Email").SetElementName("Email");
            cm.MapField("Phone").SetElementName("Phone");
        });

        // Register custom serializers for Value Objects if needed
    }
}
