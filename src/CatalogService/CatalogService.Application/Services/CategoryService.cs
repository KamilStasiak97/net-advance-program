// <copyright file="CategoryService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CatalogService.Application.Services;

using CatalogService.Application.DTOs;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Repositories;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await categoryRepository.GetAllAsync().ConfigureAwait(false);
        return categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name, Description = c.Description });
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var c = await categoryRepository.GetByIdAsync(id).ConfigureAwait(false);
        if (c == null)
        {
            return null;
        }

        return new CategoryDto { Id = c.Id, Name = c.Name, Description = c.Description };
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto)
    {
        var category = new Category { Name = categoryDto.Name, Description = categoryDto.Description };
        var created = await categoryRepository.AddAsync(category).ConfigureAwait(false);
        return new CategoryDto { Id = created.Id, Name = created.Name, Description = created.Description };
    }

    public async Task UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto)
    {
        var existing = await categoryRepository.GetByIdAsync(id).ConfigureAwait(false);
        if (existing == null)
        {
            throw new KeyNotFoundException("Category not found");
        }

        existing.Name = categoryDto.Name;
        existing.Description = categoryDto.Description;
        await categoryRepository.UpdateAsync(existing).ConfigureAwait(false);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        await categoryRepository.DeleteAsync(id).ConfigureAwait(false);
    }
}
