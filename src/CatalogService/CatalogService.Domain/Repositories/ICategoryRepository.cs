// <copyright file="ICategoryRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CatalogService.Domain.Repositories;

using CatalogService.Domain.Entities;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();

    Task<Category?> GetByIdAsync(int id);

    Task<Category> AddAsync(Category category);

    Task UpdateAsync(Category category);

    Task DeleteAsync(int id);
}
