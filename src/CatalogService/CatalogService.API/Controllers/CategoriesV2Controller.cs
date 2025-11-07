using CatalogService.Application.DTOs;
using CatalogService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
public class CategoriesV2Controller : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesV2Controller(ICategoryService categoryService)
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
        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCategoryDto dto)
    {
        var created = await _categoryService.CreateCategoryAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id, version = "2.0" }, created);
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
