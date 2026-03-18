using MediatR;

namespace SmartStock.Application.Statistics.Queries;

public record TodaySalesQuery : IRequest<TodaySalesDto>;

public class TodaySalesQueryHandler : IRequestHandler<TodaySalesQuery, TodaySalesDto>
{
    private readonly IStatisticsService _stats;

    public TodaySalesQueryHandler(IStatisticsService stats)
    {
        _stats = stats;
    }

    public Task<TodaySalesDto> Handle(TodaySalesQuery request, CancellationToken cancellationToken)
        => _stats.GetTodaySalesAsync(cancellationToken);
}

