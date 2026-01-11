// <copyright file="IProductRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CatalogService.Domain.Repositories;

using CatalogService.Domain.Entities;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync(int? categoryId, int pageNumber, int pageSize);

    Task<Product?> GetByIdAsync(int id);

    Task<Product> AddAsync(Product product);

    Task UpdateAsync(Product product);

    Task DeleteAsync(int id);
}
