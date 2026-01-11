// <copyright file="ICategoryService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CatalogService.Application.Services;

using CatalogService.Application.DTOs;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();

    Task<CategoryDto?> GetCategoryByIdAsync(int id);

    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto);

    Task UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto);

    Task DeleteCategoryAsync(int id);
}
