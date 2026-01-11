// <copyright file="IProductService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CatalogService.Application.Services;

using CatalogService.Application.DTOs;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync(int? categoryId, int pageNumber, int pageSize);

    Task<ProductDto?> GetProductByIdAsync(int id);

    Task<ProductDto> CreateProductAsync(CreateProductDto productDto);

    Task UpdateProductAsync(int id, UpdateProductDto productDto);

    Task DeleteProductAsync(int id);
}
