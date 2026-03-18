using MediatR;
using SmartStock.Domain.Enums;

namespace SmartStock.Application.POS.CreateSale;

public record CreateSaleItemInput(Guid ProductId, decimal Quantity, decimal? UnitPrice);

public record CreateSaleCommand(
    PaymentType PaymentType,
    IReadOnlyList<CreateSaleItemInput> Items,
    string? CustomerName,
    string? CustomerPhone
) : IRequest<Guid>;

