using FluentValidation;

namespace SmartStock.Application.Debts.PayDebt;

public class PayDebtCommandValidator : AbstractValidator<PayDebtCommand>
{
    public PayDebtCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.AmountPaid).GreaterThan(0);
    }
}

