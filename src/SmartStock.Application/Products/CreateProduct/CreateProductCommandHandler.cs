using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartStock.Application.Common.Interfaces;
using SmartStock.Domain.Entities;

namespace SmartStock.Application.Products.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IAppDbContext _db;

    public CreateProductCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var exists = await _db.Products.AnyAsync(x => x.Barcode == request.Barcode, cancellationToken);
        if (exists)
            throw new InvalidOperationException("Barcode already exists.");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Barcode = request.Barcode.Trim(),
            CostPrice = request.CostPrice,
            SalePrice = request.SalePrice,
            CategoryId = request.CategoryId
        };

        _db.Products.Add(product);
        _db.Stocks.Add(new SmartStock.Domain.Entities.Stock
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Quantity = 0m,
            MinStockLimit = request.MinStockLimit
        });

        await _db.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}

