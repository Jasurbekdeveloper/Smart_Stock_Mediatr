using SmartStock.Domain.Common;

namespace SmartStock.Domain.Entities;

public class AuditLog : Entity<Guid>
{
    public DateTime DateUtc { get; set; } = DateTime.UtcNow;
    public string Action { get; set; } = null!;
    public string EntityName { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
}

