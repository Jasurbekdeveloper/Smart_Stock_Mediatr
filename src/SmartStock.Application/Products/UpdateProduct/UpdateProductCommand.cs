using MediatR;

namespace SmartStock.Application.Products.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Barcode,
    decimal CostPrice,
    decimal SalePrice,
    Guid? CategoryId,
    decimal MinStockLimit
) : IRequest;

