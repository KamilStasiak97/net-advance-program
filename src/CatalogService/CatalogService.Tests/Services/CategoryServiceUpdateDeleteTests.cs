// <copyright file="CategoryServiceUpdateDeleteTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CatalogService.Tests.Services;

using CatalogService.Application.DTOs;
using CatalogService.Application.Services;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Repositories;
using FluentAssertions;
using Moq;

public class CategoryServiceUpdateDeleteTests
{
    [Fact]
    public async Task UpdateCategory_WhenExists_Calls_Update()
    {
        var repoMock = new Mock<ICategoryRepository>();
        var existing = new Category { Id = 10, Name = "Old", Description = "old" };
        repoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(existing);
        repoMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

        var service = new CategoryService(repoMock.Object);
        var dto = new UpdateCategoryDto { Name = "New", Description = "new" };

        await service.UpdateCategoryAsync(10, dto).ConfigureAwait(false);

        repoMock.Verify(r => r.UpdateAsync(It.Is<Category>(c => c.Name == "New" && c.Description == "new")), Times.Once);
    }

    [Fact]
    public async Task DeleteCategory_Calls_Repository_Delete()
    {
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.DeleteAsync(5)).Returns(Task.CompletedTask);

        var service = new CategoryService(repoMock.Object);
        await service.DeleteCategoryAsync(5).ConfigureAwait(false);

        repoMock.Verify(r => r.DeleteAsync(5), Times.Once);
    }
}
