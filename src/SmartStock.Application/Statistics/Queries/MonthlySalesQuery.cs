using MediatR;

namespace SmartStock.Application.Statistics.Queries;

public record MonthlySalesQuery(int Year) : IRequest<IReadOnlyList<MonthlySalesPointDto>>;

public class MonthlySalesQueryHandler : IRequestHandler<MonthlySalesQuery, IReadOnlyList<MonthlySalesPointDto>>
{
    private readonly IStatisticsService _stats;

    public MonthlySalesQueryHandler(IStatisticsService stats)
    {
        _stats = stats;
    }

    public Task<IReadOnlyList<MonthlySalesPointDto>> Handle(MonthlySalesQuery request, CancellationToken cancellationToken)
        => _stats.GetMonthlySalesAsync(request.Year, cancellationToken);
}

