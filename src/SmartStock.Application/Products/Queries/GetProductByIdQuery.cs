using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartStock.Application.Common.Exceptions;
using SmartStock.Application.Common.Interfaces;

namespace SmartStock.Application.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IAppDbContext _db;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IAppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _db.Products
            .Include(x => x.Stock)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (product is null)
            throw new NotFoundException("Product not found.");

        return _mapper.Map<ProductDto>(product);
    }
}

