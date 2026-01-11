// <copyright file="ProductServiceUpdateDeleteTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CatalogService.Tests.Services;

using CatalogService.Application.DTOs;
using CatalogService.Application.Services;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Repositories;
using FluentAssertions;
using MassTransit;
using Moq;

public class ProductServiceUpdateDeleteTests
{
    [Fact]
    public async Task UpdateProduct_WhenExists_Calls_Update()
    {
        var repoMock = new Mock<IProductRepository>();
        var existing = new Product { Id = 20, Name = "OldP", Description = "old", Price = 1m, CategoryId = 1 };
        repoMock.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(existing);
        repoMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

        var publishEndpointMock = new Mock<IPublishEndpoint>();
        var service = new ProductService(repoMock.Object, publishEndpointMock.Object);
        var dto = new UpdateProductDto { Name = "NewP", Description = "new", Price = 2m, CategoryId = 1 };

        await service.UpdateProductAsync(20, dto).ConfigureAwait(false);

        repoMock.Verify(r => r.UpdateAsync(It.Is<Product>(p => p.Name == "NewP" && p.Price == 2m)), Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_Calls_Repository_Delete()
    {
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(r => r.DeleteAsync(7)).Returns(Task.CompletedTask);

        var publishEndpointMock = new Mock<IPublishEndpoint>();
        var service = new ProductService(repoMock.Object, publishEndpointMock.Object);
        await service.DeleteProductAsync(7).ConfigureAwait(false);

        repoMock.Verify(r => r.DeleteAsync(7), Times.Once);
    }
}
