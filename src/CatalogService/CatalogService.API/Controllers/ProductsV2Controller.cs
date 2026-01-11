// <copyright file="ProductsV2Controller.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CatalogService.API.Controllers;

using CatalogService.Application.DTOs;
using CatalogService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
[Authorize]
public class ProductsV2Controller : ControllerBase
{
    private readonly IProductService productService;

    public ProductsV2Controller(IProductService productService)
    {
        this.productService = productService;
    }

    // V2: GET returns a plain list of items (no HATEOAS wrapper) - accessible for all authenticated users
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] int? categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var products = await productService.GetProductsAsync(categoryId, page, pageSize).ConfigureAwait(false);
        return this.Ok(products);
    }

    // V2: GET by id returns product directly (no links) - accessible for all authenticated users
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(int id)
    {
        var product = await productService.GetProductByIdAsync(id).ConfigureAwait(false);
        if (product == null)
        {
            return this.NotFound();
        }

        return this.Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Post([FromBody] CreateProductDto dto)
    {
        var created = await productService.CreateProductAsync(dto).ConfigureAwait(false);
        return this.CreatedAtAction(nameof(this.Get), new { id = created.Id, version = "2.0" }, created);
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
