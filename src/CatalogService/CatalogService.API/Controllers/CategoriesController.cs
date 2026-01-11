// <copyright file="CategoriesController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CatalogService.API.Controllers;

using CatalogService.Application.DTOs;
using CatalogService.Application.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        this.categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var categories = await categoryService.GetAllCategoriesAsync().ConfigureAwait(false);
        return this.Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var category = await categoryService.GetCategoryByIdAsync(id).ConfigureAwait(false);
        if (category == null)
        {
            return this.NotFound();
        }

        var links = new[]
        {
            new { rel = "self", href = this.Url.Action(nameof(this.Get), new { id = category.Id, version = "1.0" }), method = "GET" },
            new { rel = "update", href = this.Url.Action(nameof(this.Put), new { id = category.Id, version = "1.0" }), method = "PUT" },
            new { rel = "delete", href = this.Url.Action(nameof(this.Delete), new { id = category.Id, version = "1.0" }), method = "DELETE" },
            new { rel = "all", href = this.Url.Action(nameof(this.Get), new { version = "1.0" }), method = "GET" },
        };

        return this.Ok(new { data = category, links });
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCategoryDto dto)
    {
        var created = await categoryService.CreateCategoryAsync(dto).ConfigureAwait(false);
        return this.CreatedAtAction(nameof(this.Get), new { id = created.Id, version = "1.0" }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateCategoryDto dto)
    {
        await categoryService.UpdateCategoryAsync(id, dto).ConfigureAwait(false);
        return this.NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await categoryService.DeleteCategoryAsync(id).ConfigureAwait(false);
        return this.NoContent();
    }
}
