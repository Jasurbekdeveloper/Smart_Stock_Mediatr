using MediatR;

namespace SmartStock.Application.Stock.StockIn;

public record StockInCommand(
    Guid ProductId,
    decimal Quantity,
    string? SupplierName
) : IRequest;

