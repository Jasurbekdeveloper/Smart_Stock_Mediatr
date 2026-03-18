using SmartStock.Domain.Common;

namespace SmartStock.Domain.Entities;

public class Customer : Entity<Guid>
{
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }

    public decimal TotalDebtAmount { get; set; }

    public Debt? Debt { get; set; }
    public ICollection<DebtPayment> DebtPayments { get; set; } = new List<DebtPayment>();
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}

