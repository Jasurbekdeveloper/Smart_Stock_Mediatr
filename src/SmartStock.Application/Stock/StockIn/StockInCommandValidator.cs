using FluentValidation;

namespace SmartStock.Application.Stock.StockIn;

public class StockInCommandValidator : AbstractValidator<StockInCommand>
{
    public StockInCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.SupplierName).MaximumLength(200);
    }
}

