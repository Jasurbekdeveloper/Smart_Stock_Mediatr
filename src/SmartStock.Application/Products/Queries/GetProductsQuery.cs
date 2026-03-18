using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartStock.Application.Common.Interfaces;

namespace SmartStock.Application.Products.Queries;

public record GetProductsQuery(int Page = 1, int PageSize = 50) : IRequest<IReadOnlyList<ProductDto>>;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly IAppDbContext _db;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IAppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 200 ? 50 : request.PageSize;

        var products = await _db.Products
            .Include(x => x.Stock)
            .AsNoTracking()
            .OrderByDescending(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return products.Select(p => _mapper.Map<ProductDto>(p)).ToList();
    }
}

