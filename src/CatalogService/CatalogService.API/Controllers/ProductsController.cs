using CatalogService.Application.DTOs;
using CatalogService.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CatalogService.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] int? categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var products = await _productService.GetProductsAsync(categoryId, page, pageSize);
        return Ok(products);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        var links = new[]
        {
            new { rel = "self", href = Url.Action(nameof(Get), new { id = product.Id, version = "1.0" }), method = "GET" },
            new { rel = "update", href = Url.Action(nameof(Put), new { id = product.Id, version = "1.0" }), method = "PUT" },
            new { rel = "delete", href = Url.Action(nameof(Delete), new { id = product.Id, version = "1.0" }), method = "DELETE" },
            new { rel = "list", href = Url.Action(nameof(Get), new { version = "1.0" }), method = "GET" }
        };

        return Ok(new { data = product, links });
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Post([FromBody] CreateProductDto dto)
    {
        var created = await _productService.CreateProductAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id, version = "1.0" }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateProductDto dto)
    {
        await _productService.UpdateProductAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
}