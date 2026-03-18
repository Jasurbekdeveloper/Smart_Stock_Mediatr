using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartStock.Application.Common.Interfaces;
using SmartStock.Domain.Entities;
using SmartStock.Domain.Enums;
using SmartStock.Domain.Identity;

namespace SmartStock.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Debt> Debts => Set<Debt>();
    public DbSet<DebtPayment> DebtPayments => Set<DebtPayment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>(b =>
        {
            b.HasIndex(x => x.Barcode).IsUnique();
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.Property(x => x.Barcode).HasMaxLength(64).IsRequired();
            b.Property(x => x.CostPrice).HasPrecision(18, 2);
            b.Property(x => x.SalePrice).HasPrecision(18, 2);
        });

        builder.Entity<Category>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(120).IsRequired();
        });

        builder.Entity<Stock>(b =>
        {
            b.HasIndex(x => x.ProductId).IsUnique();
            b.Property(x => x.Quantity).HasPrecision(18, 3);
            b.Property(x => x.MinStockLimit).HasPrecision(18, 3);
        });

        builder.Entity<StockMovement>(b =>
        {
            b.Property(x => x.Quantity).HasPrecision(18, 3);
            b.Property(x => x.Type).HasConversion<int>();
            b.Property(x => x.SupplierName).HasMaxLength(200);
        });

        builder.Entity<Sale>(b =>
        {
            b.Property(x => x.TotalSum).HasPrecision(18, 2);
            b.Property(x => x.PaymentType).HasConversion<int>();
            b.Property(x => x.Status).HasConversion<int>();
        });

        builder.Entity<SaleItem>(b =>
        {
            b.Property(x => x.Quantity).HasPrecision(18, 3);
            b.Property(x => x.UnitPrice).HasPrecision(18, 2);
            b.Property(x => x.LineTotal).HasPrecision(18, 2);
        });

        builder.Entity<Customer>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.Property(x => x.Phone).HasMaxLength(32);
            b.Property(x => x.TotalDebtAmount).HasPrecision(18, 2);
        });

        builder.Entity<Debt>(b =>
        {
            b.HasIndex(x => x.CustomerId).IsUnique();
            b.Property(x => x.TotalDebtAmount).HasPrecision(18, 2);
        });

        builder.Entity<DebtPayment>(b =>
        {
            b.Property(x => x.AmountPaid).HasPrecision(18, 2);
        });

        builder.Entity<AuditLog>(b =>
        {
            b.Property(x => x.Action).HasMaxLength(80).IsRequired();
            b.Property(x => x.EntityName).HasMaxLength(80).IsRequired();
            b.Property(x => x.EntityId).HasMaxLength(80).IsRequired();
            b.Property(x => x.UserId).HasMaxLength(80);
            b.Property(x => x.UserName).HasMaxLength(256);
        });
    }
}

