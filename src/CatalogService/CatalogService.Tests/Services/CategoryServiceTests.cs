using CatalogService.Application.DTOs;
using CatalogService.Application.Services;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace CatalogService.Tests.Services;

public class CategoryServiceTests
{
    [Fact]
    public async Task CreateCategory_Should_Call_Repository_And_Return_Dto()
    {
        // Arrange
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync((Category c) => { c.Id = 42; return c; });

        var service = new CategoryService(repoMock.Object);
        var dto = new CreateCategoryDto { Name = "Toys", Description = "Kids toys" };

        // Act
        var result = await service.CreateCategoryAsync(dto);

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
        var result = await service.GetAllCategoriesAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(1);
    }
}