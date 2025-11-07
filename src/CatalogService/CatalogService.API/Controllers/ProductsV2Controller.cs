using CatalogService.Application.DTOs;
using CatalogService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
public class ProductsV2Controller : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsV2Controller(IProductService productService)
    {
        _productService = productService;
    }

    // V2: GET returns a plain list of items (no HATEOAS wrapper)
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var products = await _productService.GetProductsAsync(categoryId, page, pageSize);
        return Ok(products);
    }

    // V2: GET by id returns product directly (no links)
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateProductDto dto)
    {
        var created = await _productService.CreateProductAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id, version = "2.0" }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateProductDto dto)
    {
        await _productService.UpdateProductAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
}
