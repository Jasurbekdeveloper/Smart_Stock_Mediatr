using SmartStock.Domain.Common;

namespace SmartStock.Domain.Entities;

public class Product : Entity<Guid>
{
    public string Name { get; set; } = null!;
    public string Barcode { get; set; } = null!;
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }

    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    public Stock? Stock { get; set; }
}
