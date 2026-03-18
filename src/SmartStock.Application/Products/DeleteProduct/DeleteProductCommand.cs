using MediatR;

namespace SmartStock.Application.Products.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest;

