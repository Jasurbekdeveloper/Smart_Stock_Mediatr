namespace SmartStock.Application.Statistics;

public interface IStatisticsService
{
    Task<TodaySalesDto> GetTodaySalesAsync(CancellationToken cancellationToken);
    Task<TopSoldProductDto?> GetTopSoldProductAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken);
    Task<IReadOnlyList<MonthlySalesPointDto>> GetMonthlySalesAsync(int year, CancellationToken cancellationToken);
}

