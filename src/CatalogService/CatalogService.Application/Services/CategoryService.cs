using CatalogService.Application.DTOs;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Repositories;

namespace CatalogService.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name, Description = c.Description });
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var c = await _categoryRepository.GetByIdAsync(id);
        if (c == null) return null;
        return new CategoryDto { Id = c.Id, Name = c.Name, Description = c.Description };
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto)
    {
        var category = new Category { Name = categoryDto.Name, Description = categoryDto.Description };
        var created = await _categoryRepository.AddAsync(category);
        return new CategoryDto { Id = created.Id, Name = created.Name, Description = created.Description };
    }

    public async Task UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto)
    {
        var existing = await _categoryRepository.GetByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException("Category not found");
        existing.Name = categoryDto.Name;
        existing.Description = categoryDto.Description;
        await _categoryRepository.UpdateAsync(existing);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        await _categoryRepository.DeleteAsync(id);
    }
}