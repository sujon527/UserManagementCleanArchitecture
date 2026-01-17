using UserManagement.Domain.Common;

namespace UserManagement.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Action { get; private set; }
    public string Details { get; private set; }
    public string IpAddress { get; private set; }
    public string DeviceInfo { get; private set; }
    public string ActorId { get; private set; }

    public AuditLog(Guid userId, string action, string details, string ipAddress, string deviceInfo, string actorId)
    {
        UserId = userId;
        Action = action;
        Details = details;
        IpAddress = ipAddress;
        DeviceInfo = deviceInfo;
        ActorId = actorId;
    }
}
