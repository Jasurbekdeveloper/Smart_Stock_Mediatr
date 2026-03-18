using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartStock.Application.Common.Exceptions;
using SmartStock.Application.Common.Interfaces;

namespace SmartStock.Application.Products.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IAppDbContext _db;

    public DeleteProductCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (product is null)
            throw new NotFoundException("Product not found.");

        var stock = await _db.Stocks.FirstOrDefaultAsync(x => x.ProductId == request.Id, cancellationToken);
        if (stock is not null) _db.Stocks.Remove(stock);

        _db.Products.Remove(product);
        await _db.SaveChangesAsync(cancellationToken);
    }
}

