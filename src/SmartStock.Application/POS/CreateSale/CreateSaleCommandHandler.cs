using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartStock.Application.Common.Exceptions;
using SmartStock.Application.Common.Interfaces;
using SmartStock.Domain.Entities;
using SmartStock.Domain.Enums;

namespace SmartStock.Application.POS.CreateSale;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Guid>
{
    private readonly IAppDbContext _db;

    public CreateSaleCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var productIds = request.Items.Select(x => x.ProductId).Distinct().ToList();
        var products = await _db.Products
            .Include(p => p.Stock)
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        if (products.Count != productIds.Count)
            throw new NotFoundException("One or more products not found.");

        Customer? customer = null;
        if (request.PaymentType == PaymentType.Debt)
        {
            var phone = request.CustomerPhone!.Trim();
            customer = await _db.Customers.Include(c => c.Debt)
                .FirstOrDefaultAsync(c => c.Phone == phone, cancellationToken);

            if (customer is null)
            {
                customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    Name = request.CustomerName!.Trim(),
                    Phone = phone,
                    TotalDebtAmount = 0m
                };
                _db.Customers.Add(customer);

                var debt = new Debt
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer.Id,
                    TotalDebtAmount = 0m
                };
                _db.Debts.Add(debt);
                customer.Debt = debt;
            }
        }

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleDateUtc = DateTime.UtcNow,
            PaymentType = request.PaymentType,
            Status = SaleStatus.Completed,
            CustomerId = customer?.Id
        };

        decimal total = 0m;

        foreach (var input in request.Items)
        {
            var product = products.First(p => p.Id == input.ProductId);
            var unitPrice = input.UnitPrice ?? product.SalePrice;

            var stock = product.Stock;
            if (stock is null)
                throw new InvalidOperationException($"Stock not initialized for product {product.Name}.");

            if (stock.Quantity < input.Quantity)
                throw new InvalidOperationException($"Not enough stock for product {product.Name}.");

            stock.Quantity -= input.Quantity;

            var lineTotal = unitPrice * input.Quantity;
            total += lineTotal;

            sale.Items.Add(new SaleItem
            {
                Id = Guid.NewGuid(),
                SaleId = sale.Id,
                ProductId = product.Id,
                Quantity = input.Quantity,
                UnitPrice = unitPrice,
                LineTotal = lineTotal
            });

            _db.StockMovements.Add(new StockMovement
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Type = StockMovementType.Out,
                Quantity = input.Quantity,
                DateUtc = DateTime.UtcNow,
                SaleId = sale.Id
            });
        }

        sale.TotalSum = total;
        _db.Sales.Add(sale);

        if (request.PaymentType == PaymentType.Debt && customer is not null)
        {
            customer.TotalDebtAmount += total;
            if (customer.Debt is null)
            {
                var debt = new Debt
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer.Id,
                    TotalDebtAmount = customer.TotalDebtAmount
                };
                _db.Debts.Add(debt);
                customer.Debt = debt;
            }
            else
            {
                customer.Debt.TotalDebtAmount = customer.TotalDebtAmount;
                customer.Debt.UpdatedAtUtc = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return sale.Id;
    }
}

