using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using SmartStock.Application.Products;

namespace SmartStock.Application.Mapping;

public static class MapperFactory
{
    public static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductMappingProfile>();
        }, NullLoggerFactory.Instance);

        return config.CreateMapper();
    }
}

