// <copyright file="CategoryIntegrationTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CatalogService.Tests.Integration;

using CatalogService.Domain.Entities;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

public class CategoryIntegrationTests
{
    [Fact]
    public async Task CategoryRepository_Add_And_Get_Works_With_InMemory_Db()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new CatalogDbContext(options);
        var repo = new CategoryRepository(context);

        var category = new Category { Name = "Sports", Description = "Sporting goods" };
        var created = await repo.AddAsync(category).ConfigureAwait(false);

        created.Id.Should().BeGreaterThan(0);

        var fetched = await repo.GetByIdAsync(created.Id).ConfigureAwait(false);
        fetched.Should().NotBeNull();
        fetched!.Name.Should().Be("Sports");
    }
}
