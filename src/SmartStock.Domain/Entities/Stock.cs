using SmartStock.Domain.Common;

namespace SmartStock.Domain.Entities;

public class Stock : Entity<Guid>
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public decimal Quantity { get; set; }
    public decimal MinStockLimit { get; set; }
}

