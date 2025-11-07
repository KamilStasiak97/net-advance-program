using CatalogService.Domain.Entities;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Tests.Integration;

public class ProductRepositoryTests
{
    [Fact]
    public async Task ProductRepository_CRUD_And_Filtering_Pagination_Work()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new CatalogDbContext(options);

        var cat1 = new Category { Name = "C1", Description = "d" };
        var cat2 = new Category { Name = "C2", Description = "d" };
        context.Categories.AddRange(cat1, cat2);
        await context.SaveChangesAsync();

        var p1 = new Product { Name = "P1", Description = "d1", Price = 1m, CategoryId = cat1.Id };
        var p2 = new Product { Name = "P2", Description = "d2", Price = 2m, CategoryId = cat1.Id };
        var p3 = new Product { Name = "P3", Description = "d3", Price = 3m, CategoryId = cat2.Id };
        context.Products.AddRange(p1, p2, p3);
        await context.SaveChangesAsync();

        var repo = new ProductRepository(context);

        // Filtering by categoryId
        var cat1Products = (await repo.GetAllAsync(cat1.Id, 1, 10)).ToList();
        cat1Products.Should().HaveCount(2);

        // Pagination: pageSize=1 should return 1 item for page 2
        var page2 = (await repo.GetAllAsync(null, 2, 1)).ToList();
        page2.Should().HaveCount(1);

        // GetById
        var fetched = await repo.GetByIdAsync(p2.Id);
        fetched.Should().NotBeNull();
        fetched!.Name.Should().Be("P2");

        // Update
        fetched.Name = "P2-upd";
        await repo.UpdateAsync(fetched);
        var updated = await repo.GetByIdAsync(fetched.Id);
        updated!.Name.Should().Be("P2-upd");

        // Delete
        await repo.DeleteAsync(p1.Id);
        var afterDelete = await repo.GetByIdAsync(p1.Id);
        afterDelete.Should().BeNull();
    }
}
