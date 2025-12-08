using CatalogService.Application.DTOs;
using CatalogService.Application.Services;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace CatalogService.Tests.Services;

public class ProductServiceTests
{
    [Fact]
    public async Task GetProducts_Should_Return_Mapped_Dtos_With_Pagination()
    {
        // Arrange
        var products = new[]
        {
            new Product { Id = 1, Name = "P1", Description = "d1", Price = 1m, CategoryId = 1, Category = new Category { Id = 1, Name = "C1" } },
            new Product { Id = 2, Name = "P2", Description = "d2", Price = 2m, CategoryId = 1, Category = new Category { Id = 1, Name = "C1" } }
        };

        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.GetAllAsync(It.IsAny<int?>(), 1, 1)).ReturnsAsync(new[] { products[0] });

        var service = new ProductService(repoMock.Object);

        // Act
        var result = (await service.GetProductsAsync(null, 1, 1)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(1);
        result[0].Category.Should().NotBeNull();
        repoMock.Verify(r => r.GetAllAsync(null, 1, 1), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_Should_Call_Repository_And_Return_Dto()
    {
        // Arrange
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.AddAsync(It.IsAny<Product>())).ReturnsAsync((Product p) => { p.Id = 99; return p; });

        var service = new ProductService(repoMock.Object);
        var dto = new CreateProductDto { Name = "New", Description = "desc", Price = 12.5m, CategoryId = 1 };

        // Act
        var created = await service.CreateProductAsync(dto);

        // Assert
        created.Should().NotBeNull();
        created.Id.Should().Be(99);
        created.Name.Should().Be(dto.Name);
        repoMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
    }
}
