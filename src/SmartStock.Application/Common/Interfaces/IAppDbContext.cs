using Microsoft.EntityFrameworkCore;
using SmartStock.Domain.Entities;

namespace SmartStock.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<SmartStock.Domain.Entities.Stock> Stocks { get; }
    DbSet<StockMovement> StockMovements { get; }
    DbSet<Sale> Sales { get; }
    DbSet<SaleItem> SaleItems { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Debt> Debts { get; }
    DbSet<DebtPayment> DebtPayments { get; }
    DbSet<AuditLog> AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

