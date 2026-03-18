using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartStock.Application.Products.CreateProduct;
using SmartStock.Application.Products.DeleteProduct;
using SmartStock.Application.Products.Queries;
using SmartStock.Application.Products.UpdateProduct;
using SmartStock.Domain.Identity;

namespace SmartStock.WebAPI.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SmartStock.Application.Products.ProductDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        => Ok(await _mediator.Send(new GetProductsQuery(page, pageSize)));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SmartStock.Application.Products.ProductDto>> Get(Guid id)
        => Ok(await _mediator.Send(new GetProductByIdQuery(id)));

    [HttpGet("by-barcode/{barcode}")]
    public async Task<ActionResult<SmartStock.Application.Products.ProductDto>> GetByBarcode(string barcode)
        => Ok(await _mediator.Send(new SearchProductByBarcodeQuery(barcode)));

    public record CreateProductRequest(string Name, string Barcode, decimal CostPrice, decimal SalePrice, Guid? CategoryId, decimal MinStockLimit);

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create(CreateProductRequest request)
    {
        var id = await _mediator.Send(new CreateProductCommand(
            request.Name,
            request.Barcode,
            request.CostPrice,
            request.SalePrice,
            request.CategoryId,
            request.MinStockLimit));

        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    public record UpdateProductRequest(string Name, string Barcode, decimal CostPrice, decimal SalePrice, Guid? CategoryId, decimal MinStockLimit);

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id, UpdateProductRequest request)
    {
        await _mediator.Send(new UpdateProductCommand(
            id,
            request.Name,
            request.Barcode,
            request.CostPrice,
            request.SalePrice,
            request.CategoryId,
            request.MinStockLimit));

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteProductCommand(id));
        return NoContent();
    }
}

