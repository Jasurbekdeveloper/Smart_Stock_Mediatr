using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartStock.Application.Common.Exceptions;
using SmartStock.Application.Common.Interfaces;
using SmartStock.Domain.Entities;
using SmartStock.Domain.Enums;

namespace SmartStock.Application.Stock.StockIn;

public class StockInCommandHandler : IRequestHandler<StockInCommand>
{
    private readonly IAppDbContext _db;

    public StockInCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task Handle(StockInCommand request, CancellationToken cancellationToken)
    {
        var productExists = await _db.Products.AnyAsync(x => x.Id == request.ProductId, cancellationToken);
        if (!productExists)
            throw new NotFoundException("Product not found.");

        var stock = await _db.Stocks.FirstOrDefaultAsync(x => x.ProductId == request.ProductId, cancellationToken);
        if (stock is null)
        {
            stock = new SmartStock.Domain.Entities.Stock
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                Quantity = 0m,
                MinStockLimit = 0m
            };
            _db.Stocks.Add(stock);
        }

        stock.Quantity += request.Quantity;

        _db.StockMovements.Add(new StockMovement
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            Type = StockMovementType.In,
            Quantity = request.Quantity,
            SupplierName = string.IsNullOrWhiteSpace(request.SupplierName) ? null : request.SupplierName.Trim(),
            DateUtc = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(cancellationToken);
    }
}

