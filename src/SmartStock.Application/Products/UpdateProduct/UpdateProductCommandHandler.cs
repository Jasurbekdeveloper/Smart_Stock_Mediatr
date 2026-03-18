using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartStock.Application.Common.Exceptions;
using SmartStock.Application.Common.Interfaces;

namespace SmartStock.Application.Products.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IAppDbContext _db;

    public UpdateProductCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _db.Products
            .Include(x => x.Stock)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (product is null)
            throw new NotFoundException("Product not found.");

        var barcodeTaken = await _db.Products.AnyAsync(x => x.Barcode == request.Barcode && x.Id != request.Id, cancellationToken);
        if (barcodeTaken)
            throw new InvalidOperationException("Barcode already exists.");

        product.Name = request.Name.Trim();
        product.Barcode = request.Barcode.Trim();
        product.CostPrice = request.CostPrice;
        product.SalePrice = request.SalePrice;
        product.CategoryId = request.CategoryId;

        if (product.Stock is null)
        {
            _db.Stocks.Add(new SmartStock.Domain.Entities.Stock
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Quantity = 0m,
                MinStockLimit = request.MinStockLimit
            });
        }
        else
        {
            product.Stock.MinStockLimit = request.MinStockLimit;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}

