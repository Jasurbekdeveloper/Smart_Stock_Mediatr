using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartStock.Application.Common.Exceptions;
using SmartStock.Application.Common.Interfaces;
using SmartStock.Domain.Entities;

namespace SmartStock.Application.Debts.PayDebt;

public class PayDebtCommandHandler : IRequestHandler<PayDebtCommand>
{
    private readonly IAppDbContext _db;

    public PayDebtCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task Handle(PayDebtCommand request, CancellationToken cancellationToken)
    {
        var customer = await _db.Customers
            .Include(c => c.Debt)
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (customer is null)
            throw new NotFoundException("Customer not found.");

        var currentDebt = customer.TotalDebtAmount;
        if (currentDebt <= 0m)
            return;

        var payment = request.AmountPaid > currentDebt ? currentDebt : request.AmountPaid;
        customer.TotalDebtAmount = currentDebt - payment;

        if (customer.Debt is null)
        {
            customer.Debt = new Debt
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                TotalDebtAmount = customer.TotalDebtAmount,
                UpdatedAtUtc = DateTime.UtcNow
            };
            _db.Debts.Add(customer.Debt);
        }
        else
        {
            customer.Debt.TotalDebtAmount = customer.TotalDebtAmount;
            customer.Debt.UpdatedAtUtc = DateTime.UtcNow;
        }

        _db.DebtPayments.Add(new DebtPayment
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            AmountPaid = payment,
            DateUtc = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(cancellationToken);
    }
}

