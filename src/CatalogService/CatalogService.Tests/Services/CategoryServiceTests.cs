// <copyright file="CategoryServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CatalogService.Tests.Services;

using CatalogService.Application.DTOs;
using CatalogService.Application.Services;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Repositories;
using FluentAssertions;
using Moq;

public class CategoryServiceTests
{
    [Fact]
    public async Task CreateCategory_Should_Call_Repository_And_Return_Dto()
    {
        // Arrange
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync((Category c) => { c.Id = 42;
                return c; });

        var service = new CategoryService(repoMock.Object);
        var dto = new CreateCategoryDto { Name = "Toys", Description = "Kids toys" };

        // Act
        var result = await service.CreateCategoryAsync(dto).ConfigureAwait(false);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        result.Name.Should().Be(dto.Name);
        repoMock.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task GetAllCategories_Should_Return_Mapped_Dtos()
    {
        // Arrange
        var categories = new[] { new Category { Id = 1, Name = "A", Description = "d" } };
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        var service = new CategoryService(repoMock.Object);

        // Act
        var result = await service.GetAllCategoriesAsync().ConfigureAwait(false);

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(1);
    }
}
