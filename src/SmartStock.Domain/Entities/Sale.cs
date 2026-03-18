using SmartStock.Domain.Common;
using SmartStock.Domain.Enums;

namespace SmartStock.Domain.Entities;

public class Sale : Entity<Guid>
{
    public DateTime SaleDateUtc { get; set; } = DateTime.UtcNow;
    public decimal TotalSum { get; set; }
    public PaymentType PaymentType { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.Completed;

    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}

