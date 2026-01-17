using MongoDB.Driver;
using UserManagement.Domain.Entities;
using UserManagement.Domain.ValueObjects;
using UserManagement.Domain.Common;
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
        if (BsonClassMap.IsClassMapRegistered(typeof(BaseEntity))) return;

        BsonClassMap.RegisterClassMap<UserManagement.Domain.Common.BaseEntity>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(c => c.Id);
            cm.SetIsRootClass(true);
        });

        BsonClassMap.RegisterClassMap<User>(cm =>
        {
            cm.AutoMap();
            // Fields are automatically mapped by AutoMap, 
            // but we can be explicit if names differ in DB
        });

        BsonClassMap.RegisterClassMap<AuditLog>(cm =>
        {
            cm.AutoMap();
        });
    }
}
