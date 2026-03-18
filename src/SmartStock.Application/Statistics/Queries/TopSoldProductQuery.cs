using MediatR;

namespace SmartStock.Application.Statistics.Queries;

public record TopSoldProductQuery(DateTime FromUtc, DateTime ToUtc) : IRequest<TopSoldProductDto?>;

public class TopSoldProductQueryHandler : IRequestHandler<TopSoldProductQuery, TopSoldProductDto?>
{
    private readonly IStatisticsService _stats;

    public TopSoldProductQueryHandler(IStatisticsService stats)
    {
        _stats = stats;
    }

    public Task<TopSoldProductDto?> Handle(TopSoldProductQuery request, CancellationToken cancellationToken)
        => _stats.GetTopSoldProductAsync(request.FromUtc, request.ToUtc, cancellationToken);
}

