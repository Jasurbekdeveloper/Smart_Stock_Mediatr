using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartStock.Application.Statistics;
using SmartStock.Application.Statistics.Queries;
using SmartStock.Domain.Identity;

namespace SmartStock.WebAPI.Controllers;

[ApiController]
[Route("api/statistics")]
[Authorize(Roles = Roles.Admin)]
public class StatisticsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StatisticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("today-sales")]
    public async Task<ActionResult<TodaySalesDto>> TodaySales()
        => Ok(await _mediator.Send(new TodaySalesQuery()));

    [HttpGet("top-sold")]
    public async Task<ActionResult<TopSoldProductDto?>> TopSold([FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc)
    {
        var from = fromUtc ?? DateTime.UtcNow.Date;
        var to = toUtc ?? from.AddDays(1);
        return Ok(await _mediator.Send(new TopSoldProductQuery(from, to)));
    }

    [HttpGet("monthly")]
    public async Task<ActionResult<IReadOnlyList<MonthlySalesPointDto>>> Monthly([FromQuery] int? year)
        => Ok(await _mediator.Send(new MonthlySalesQuery(year ?? DateTime.UtcNow.Year)));
}

