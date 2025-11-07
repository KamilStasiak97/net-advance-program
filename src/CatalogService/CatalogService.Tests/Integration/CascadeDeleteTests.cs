using CatalogService.Domain.Entities;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Tests.Integration;

public class CascadeDeleteTests
{
    [Fact]
    public async Task Deleting_Category_Should_Remove_Related_Products()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new CatalogDbContext(options);

        var category = new Category { Name = "ToDelete", Description = "will be removed" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

    var p1 = new Product { Name = "p1", Description = "d1", Price = 1m, CategoryId = category.Id };
    var p2 = new Product { Name = "p2", Description = "d2", Price = 2m, CategoryId = category.Id };
        context.Products.AddRange(p1, p2);
        await context.SaveChangesAsync();

        // Sanity
        context.Products.Count().Should().BeGreaterThanOrEqualTo(2);

        var repo = new CategoryRepository(context);
        await repo.DeleteAsync(category.Id);

        var productsLeft = await context.Products.ToListAsync();
        productsLeft.Should().BeEmpty();
    }
}
