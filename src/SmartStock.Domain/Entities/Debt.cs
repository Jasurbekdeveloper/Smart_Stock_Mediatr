using SmartStock.Domain.Common;

namespace SmartStock.Domain.Entities;

public class Debt : Entity<Guid>
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public decimal TotalDebtAmount { get; set; }
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

