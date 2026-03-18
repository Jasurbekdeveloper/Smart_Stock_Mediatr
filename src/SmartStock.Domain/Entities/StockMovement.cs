using SmartStock.Domain.Common;
using SmartStock.Domain.Enums;

namespace SmartStock.Domain.Entities;

public class StockMovement : Entity<Guid>
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public StockMovementType Type { get; set; }
    public decimal Quantity { get; set; }
    public DateTime DateUtc { get; set; } = DateTime.UtcNow;

    public string? SupplierName { get; set; }
    public Guid? SaleId { get; set; }
    public Sale? Sale { get; set; }
}

