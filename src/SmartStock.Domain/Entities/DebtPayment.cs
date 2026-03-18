using SmartStock.Domain.Common;

namespace SmartStock.Domain.Entities;

public class DebtPayment : Entity<Guid>
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public decimal AmountPaid { get; set; }
    public DateTime DateUtc { get; set; } = DateTime.UtcNow;
}

