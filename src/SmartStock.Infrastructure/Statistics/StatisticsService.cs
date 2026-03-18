using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SmartStock.Application.Statistics;

namespace SmartStock.Infrastructure.Statistics;

public class StatisticsService : IStatisticsService
{
    private readonly string _connectionString;

    public StatisticsService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("DefaultConnection is not configured.");
    }

    private NpgsqlConnection CreateConnection() => new(_connectionString);

    public async Task<TodaySalesDto> GetTodaySalesAsync(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var from = today.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var to = from.AddDays(1);

        const string sql = """
                           select count(*) as SalesCount,
                                  coalesce(sum("TotalSum"), 0) as TotalSum
                           from "Sales"
                           where "SaleDateUtc" >= @from and "SaleDateUtc" < @to
                             and "Status" = 1
                           """;

        await using var conn = CreateConnection();
        var row = await conn.QuerySingleAsync<(int SalesCount, decimal TotalSum)>(
            new CommandDefinition(sql, new { from, to }, cancellationToken: cancellationToken));

        return new TodaySalesDto(today, row.SalesCount, row.TotalSum);
    }

    public async Task<TopSoldProductDto?> GetTopSoldProductAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken)
    {
        const string sql = """
                           select si."ProductId" as ProductId,
                                  p."Name" as ProductName,
                                  sum(si."Quantity") as QuantitySold
                           from "SaleItems" si
                           join "Sales" s on s."Id" = si."SaleId"
                           join "Products" p on p."Id" = si."ProductId"
                           where s."SaleDateUtc" >= @fromUtc and s."SaleDateUtc" < @toUtc
                             and s."Status" = 1
                           group by si."ProductId", p."Name"
                           order by sum(si."Quantity") desc
                           limit 1
                           """;

        await using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<TopSoldProductDto>(
            new CommandDefinition(sql, new { fromUtc, toUtc }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<MonthlySalesPointDto>> GetMonthlySalesAsync(int year, CancellationToken cancellationToken)
    {
        var from = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = from.AddYears(1);

        const string sql = """
                           select extract(month from s."SaleDateUtc")::int as Month,
                                  coalesce(sum(s."TotalSum"), 0) as TotalSum
                           from "Sales" s
                           where s."SaleDateUtc" >= @from and s."SaleDateUtc" < @to
                             and s."Status" = 1
                           group by extract(month from s."SaleDateUtc")
                           order by Month
                           """;

        await using var conn = CreateConnection();
        var rows = (await conn.QueryAsync<MonthlySalesPointDto>(
            new CommandDefinition(sql, new { from, to }, cancellationToken: cancellationToken))).ToList();

        // Fill missing months
        var byMonth = rows.ToDictionary(r => r.Month, r => r.TotalSum);
        return Enumerable.Range(1, 12)
            .Select(m => new MonthlySalesPointDto(m, byMonth.TryGetValue(m, out var sum) ? sum : 0m))
            .ToList();
    }
}

