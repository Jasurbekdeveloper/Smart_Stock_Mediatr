using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartStock.Application.Common.Exceptions;
using SmartStock.Application.Common.Interfaces;

namespace SmartStock.Application.Products.Queries;

public record SearchProductByBarcodeQuery(string Barcode) : IRequest<ProductDto>;

public class SearchProductByBarcodeQueryHandler : IRequestHandler<SearchProductByBarcodeQuery, ProductDto>
{
    private readonly IAppDbContext _db;
    private readonly IMapper _mapper;

    public SearchProductByBarcodeQueryHandler(IAppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(SearchProductByBarcodeQuery request, CancellationToken cancellationToken)
    {
        var barcode = request.Barcode.Trim();
        var product = await _db.Products
            .Include(x => x.Stock)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Barcode == barcode, cancellationToken);

        if (product is null)
            throw new NotFoundException("Product not found.");

        return _mapper.Map<ProductDto>(product);
    }
}

