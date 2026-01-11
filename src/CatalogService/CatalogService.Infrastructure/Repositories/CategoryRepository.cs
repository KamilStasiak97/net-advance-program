// <copyright file="CategoryRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CatalogService.Infrastructure.Repositories;

using CatalogService.Domain.Entities;
using CatalogService.Domain.Repositories;
using CatalogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class CategoryRepository : ICategoryRepository
{
    private readonly CatalogDbContext context;

    public CategoryRepository(CatalogDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await context.Categories.Include(c => c.Products).ToListAsync().ConfigureAwait(false);
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id).ConfigureAwait(false);
    }

    public async Task<Category> AddAsync(Category category)
    {
        this.context.Categories.Add(category);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return category;
    }

    public async Task UpdateAsync(Category category)
    {
        this.context.Entry(category).State = EntityState.Modified;
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await context.Categories.FindAsync(id).ConfigureAwait(false);
        if (category != null)
        {
            this.context.Categories.Remove(category);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
