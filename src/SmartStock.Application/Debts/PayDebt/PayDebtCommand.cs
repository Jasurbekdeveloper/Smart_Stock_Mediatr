using MediatR;

namespace SmartStock.Application.Debts.PayDebt;

public record PayDebtCommand(Guid CustomerId, decimal AmountPaid) : IRequest;

