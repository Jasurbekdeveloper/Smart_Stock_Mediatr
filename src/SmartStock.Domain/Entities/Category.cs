using SmartStock.Domain.Common;

namespace SmartStock.Domain.Entities;

public class Category : Entity<Guid>
{
    public string Name { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

