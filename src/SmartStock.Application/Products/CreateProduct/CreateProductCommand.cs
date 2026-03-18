using MediatR;

namespace SmartStock.Application.Products.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Barcode,
    decimal CostPrice,
    decimal SalePrice,
    Guid? CategoryId,
    decimal MinStockLimit
) : IRequest<Guid>;

