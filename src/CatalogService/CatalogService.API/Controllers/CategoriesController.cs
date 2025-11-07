using CatalogService.Application.DTOs;
using CatalogService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null) return NotFound();
        var links = new[]
        {
            new { rel = "self", href = Url.Action(nameof(Get), new { id = category.Id, version = "1.0" }), method = "GET" },
            new { rel = "update", href = Url.Action(nameof(Put), new { id = category.Id, version = "1.0" }), method = "PUT" },
            new { rel = "delete", href = Url.Action(nameof(Delete), new { id = category.Id, version = "1.0" }), method = "DELETE" },
            new { rel = "all", href = Url.Action(nameof(Get), new { version = "1.0" }), method = "GET" }
        };

        return Ok(new { data = category, links });
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCategoryDto dto)
    {
        var created = await _categoryService.CreateCategoryAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id, version = "1.0" }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateCategoryDto dto)
    {
        await _categoryService.UpdateCategoryAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }
}