using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartStock.Application.POS.CreateSale;
using SmartStock.Domain.Enums;
using SmartStock.Domain.Identity;

namespace SmartStock.WebAPI.Controllers;

[ApiController]
[Route("api/pos")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sotuvchi}")]
public class PosController : ControllerBase
{
    private readonly IMediator _mediator;

    public PosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public record CreateSaleRequest(
        PaymentType PaymentType,
        IReadOnlyList<CreateSaleItemRequest> Items,
        string? CustomerName,
        string? CustomerPhone);

    public record CreateSaleItemRequest(Guid ProductId, decimal Quantity, decimal? UnitPrice);

    [HttpPost("sales")]
    public async Task<IActionResult> CreateSale(CreateSaleRequest request)
    {
        var id = await _mediator.Send(new CreateSaleCommand(
            request.PaymentType,
            request.Items.Select(i => new CreateSaleItemInput(i.ProductId, i.Quantity, i.UnitPrice)).ToList(),
            request.CustomerName,
            request.CustomerPhone));

        return Ok(new { id });
    }
}

