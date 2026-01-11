// <copyright file="ProductService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CatalogService.Application.Services;

using CartService.Application.Messaging;
using CatalogService.Application.DTOs;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Repositories;
using MassTransit;

public class ProductService : IProductService
{
    private readonly IProductRepository productRepository;
    private readonly IPublishEndpoint publishEndpoint;

    public ProductService(IProductRepository productRepository, IPublishEndpoint publishEndpoint)
    {
        this.productRepository = productRepository;
        this.publishEndpoint = publishEndpoint;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync(int? categoryId, int pageNumber, int pageSize)
    {
        var products = await productRepository.GetAllAsync(categoryId, pageNumber, pageSize).ConfigureAwait(false);
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            CategoryId = p.CategoryId,
            Category = p.Category != null
                ? new CategoryDto { Id = p.Category.Id, Name = p.Category.Name, Description = p.Category.Description }
                : new CategoryDto { Id = p.CategoryId, Name = string.Empty, Description = string.Empty },
        });
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var p = await productRepository.GetByIdAsync(id).ConfigureAwait(false);
        if (p == null)
        {
            return null;
        }

        return new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            CategoryId = p.CategoryId,
            Category = p.Category != null
                ? new CategoryDto { Id = p.Category.Id, Name = p.Category.Name, Description = p.Category.Description }
                : new CategoryDto { Id = p.CategoryId, Name = string.Empty, Description = string.Empty },
        };
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto productDto)
    {
        var product = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            CategoryId = productDto.CategoryId,
        };

        var created = await productRepository.AddAsync(product).ConfigureAwait(false);

        // Category navigation might be null; we'll fill minimal info
        return new ProductDto
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            Price = created.Price,
            CategoryId = created.CategoryId,
            Category = new CategoryDto { Id = created.CategoryId, Name = string.Empty, Description = string.Empty },
        };
    }

    public async Task UpdateProductAsync(int id, UpdateProductDto productDto)
    {
        var existing = await productRepository.GetByIdAsync(id).ConfigureAwait(false);
        if (existing == null)
        {
            throw new KeyNotFoundException("Product not found");
        }

        existing.Name = productDto.Name;
        existing.Description = productDto.Description;
        existing.Price = productDto.Price;
        existing.CategoryId = productDto.CategoryId;
        await productRepository.UpdateAsync(existing).ConfigureAwait(false);

        var message = new ProductUpdatedMessage
        {
            ProductId = id,
            Name = productDto.Name,
            Price = productDto.Price,
        };
        await publishEndpoint.Publish(message).ConfigureAwait(false);
    }

    public async Task DeleteProductAsync(int id)
    {
        await productRepository.DeleteAsync(id).ConfigureAwait(false);
    }
}
