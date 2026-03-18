namespace SmartStock.Application.Products;

public record ProductDto(
    Guid Id,
    string Name,
    string Barcode,
    decimal CostPrice,
    decimal SalePrice,
    Guid? CategoryId,
    decimal StockQuantity,
    decimal MinStockLimit
);

