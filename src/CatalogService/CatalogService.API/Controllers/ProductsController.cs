// <copyright file="ProductsController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CatalogService.API.Controllers;

using CatalogService.Application.DTOs;
using CatalogService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService productService;

    public ProductsController(IProductService productService)
    {
        this.productService = productService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] int? categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var products = await productService.GetProductsAsync(categoryId, page, pageSize).ConfigureAwait(false);
        return this.Ok(products);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(int id)
    {
        var product = await productService.GetProductByIdAsync(id).ConfigureAwait(false);
        if (product == null)
        {
            return this.NotFound();
        }

        var links = new[]
        {
            new { rel = "self", href = this.Url.Action(nameof(this.Get), new { id = product.Id, version = "1.0" }), method = "GET" },
            new { rel = "update", href = this.Url.Action(nameof(this.Put), new { id = product.Id, version = "1.0" }), method = "PUT" },
            new { rel = "delete", href = this.Url.Action(nameof(this.Delete), new { id = product.Id, version = "1.0" }), method = "DELETE" },
            new { rel = "list", href = this.Url.Action(nameof(this.Get), new { version = "1.0" }), method = "GET" },
        };

        return this.Ok(new { data = product, links });
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Post([FromBody] CreateProductDto dto)
    {
        var created = await productService.CreateProductAsync(dto).ConfigureAwait(false);
        return this.CreatedAtAction(nameof(this.Get), new { id = created.Id, version = "1.0" }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateProductDto dto)
    {
        await productService.UpdateProductAsync(id, dto).ConfigureAwait(false);
        return this.NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        await productService.DeleteProductAsync(id).ConfigureAwait(false);
        return this.NoContent();
    }
}
