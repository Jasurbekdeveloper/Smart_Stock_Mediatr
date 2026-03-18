using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SmartStock.Application.Common.Interfaces;
using SmartStock.Domain.Entities;

namespace SmartStock.Infrastructure.Auditing;

public class ProductAuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUser;

    public ProductAuditSaveChangesInterceptor(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        WriteAuditLogs(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        WriteAuditLogs(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void WriteAuditLogs(DbContext? context)
    {
        if (context is null) return;

        var entries = context.ChangeTracker.Entries<Product>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .ToList();

        if (entries.Count == 0) return;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                context.Set<AuditLog>().Add(new AuditLog
                {
                    Action = "ProductCreated",
                    EntityName = nameof(Product),
                    EntityId = entry.Entity.Id.ToString(),
                    NewValuesJson = JsonSerializer.Serialize(new
                    {
                        entry.Entity.Name,
                        entry.Entity.Barcode,
                        entry.Entity.CostPrice,
                        entry.Entity.SalePrice
                    }),
                    UserId = _currentUser.UserId?.ToString(),
                    UserName = _currentUser.UserName
                });
                continue;
            }

            var costChanged = entry.Property(x => x.CostPrice).IsModified;
            var saleChanged = entry.Property(x => x.SalePrice).IsModified;
            if (!costChanged && !saleChanged) continue;

            var oldValues = new Dictionary<string, object?>();
            var newValues = new Dictionary<string, object?>();

            if (costChanged)
            {
                oldValues[nameof(Product.CostPrice)] = entry.Property(x => x.CostPrice).OriginalValue;
                newValues[nameof(Product.CostPrice)] = entry.Property(x => x.CostPrice).CurrentValue;
            }

            if (saleChanged)
            {
                oldValues[nameof(Product.SalePrice)] = entry.Property(x => x.SalePrice).OriginalValue;
                newValues[nameof(Product.SalePrice)] = entry.Property(x => x.SalePrice).CurrentValue;
            }

            context.Set<AuditLog>().Add(new AuditLog
            {
                Action = "ProductPriceChanged",
                EntityName = nameof(Product),
                EntityId = entry.Entity.Id.ToString(),
                OldValuesJson = JsonSerializer.Serialize(oldValues),
                NewValuesJson = JsonSerializer.Serialize(newValues),
                UserId = _currentUser.UserId?.ToString(),
                UserName = _currentUser.UserName
            });
        }
    }
}

