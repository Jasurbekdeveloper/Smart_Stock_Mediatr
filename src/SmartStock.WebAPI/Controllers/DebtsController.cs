using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartStock.Application.Debts.PayDebt;
using SmartStock.Domain.Identity;

namespace SmartStock.WebAPI.Controllers;

[ApiController]
[Route("api/debts")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sotuvchi}")]
public class DebtsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DebtsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public record PayDebtRequest(Guid CustomerId, decimal AmountPaid);

    [HttpPost("payments")]
    public async Task<IActionResult> Pay(PayDebtRequest request)
    {
        await _mediator.Send(new PayDebtCommand(request.CustomerId, request.AmountPaid));
        return NoContent();
    }
}

