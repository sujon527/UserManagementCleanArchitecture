namespace UserManagement.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public long Version { get; protected set; } = 1;

    public void IncrementVersion() => Version++;
    public void SetUpdatedAt() => UpdatedAt = DateTime.UtcNow;
}
