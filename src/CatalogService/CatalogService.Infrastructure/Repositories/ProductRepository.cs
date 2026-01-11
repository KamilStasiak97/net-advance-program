// <copyright file="ProductRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CatalogService.Infrastructure.Repositories;

using CatalogService.Domain.Entities;
using CatalogService.Domain.Repositories;
using CatalogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class ProductRepository : IProductRepository
{
    private readonly CatalogDbContext context;

    public ProductRepository(CatalogDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync(int? categoryId, int pageNumber, int pageSize)
    {
        var query = this.context.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id).ConfigureAwait(false);
    }

    public async Task<Product> AddAsync(Product product)
    {
        this.context.Products.Add(product);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        this.context.Entry(product).State = EntityState.Modified;
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await context.Products.FindAsync(id).ConfigureAwait(false);
        if (product != null)
        {
            this.context.Products.Remove(product);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
