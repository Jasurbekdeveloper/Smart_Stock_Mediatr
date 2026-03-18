using AutoMapper;
using SmartStock.Domain.Entities;

namespace SmartStock.Application.Products;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForCtorParam(nameof(ProductDto.StockQuantity),
                opt => opt.MapFrom(src => src.Stock != null ? src.Stock.Quantity : 0m))
            .ForCtorParam(nameof(ProductDto.MinStockLimit),
                opt => opt.MapFrom(src => src.Stock != null ? src.Stock.MinStockLimit : 0m));
    }
}

