using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartStock.Application.Stock.StockIn;
using SmartStock.Domain.Identity;

namespace SmartStock.WebAPI.Controllers;

[ApiController]
[Route("api/stock")]
[Authorize]
public class StockController : ControllerBase
{
    private readonly IMediator _mediator;

    public StockController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public record StockInRequest(Guid ProductId, decimal Quantity, string? SupplierName);

    [HttpPost("in")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Sotuvchi}")]
    public async Task<IActionResult> StockIn(StockInRequest request)
    {
        await _mediator.Send(new StockInCommand(request.ProductId, request.Quantity, request.SupplierName));
        return NoContent();
    }
}

