namespace SmartStock.Application.Statistics;

public record TodaySalesDto(DateOnly Date, int SalesCount, decimal TotalSum);

public record TopSoldProductDto(Guid ProductId, string ProductName, decimal QuantitySold);

public record MonthlySalesPointDto(int Month, decimal TotalSum);

