using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Xunit;
using CartService.Application.Services;
using CartService.Application.DTOs;
using CartService.Domain.Repositories;
using CartService.Domain.Entities;

namespace CartService.Tests.Services;

public class AppCartServiceTests
{
    [Fact]
    public void AddItem_CallsRepository()
    {
        var repo = new Mock<ICartRepository>();
        var svc = new CartService.Application.Services.CartService(repo.Object);

        svc.AddItem("k1", new AddCartItemDto { ProductId = 11, Quantity = 3 });

        repo.Verify(r => r.AddItem("k1", It.Is<CartItem>(ci => ci.ProductId == 11 && ci.Quantity == 3)), Times.Once);
    }

    [Fact]
    public void GetCart_MapsDomainToDto()
    {
        var repo = new Mock<ICartRepository>();
        repo.Setup(r => r.GetCart("k2")).Returns(new Cart { Key = "k2", Items = new List<CartItem> { new CartItem { Id = 5, ProductId = 7, Quantity = 2 } } });

        var svc = new CartService.Application.Services.CartService(repo.Object);
        var cart = svc.GetCart("k2");

        cart.Should().NotBeNull();
        cart!.Key.Should().Be("k2");
        cart.Items.Should().HaveCount(1);
        cart.Items[0].ProductId.Should().Be(7);
    }

    [Fact]
    public void DeleteItem_ReturnsRepositoryResult()
    {
        var repo = new Mock<ICartRepository>();
        repo.Setup(r => r.DeleteItem("k3", 9)).Returns(true);
        repo.Setup(r => r.DeleteItem("k3", 99)).Returns(false);

        var svc = new CartService.Application.Services.CartService(repo.Object);

        svc.DeleteItem("k3", 9).Should().BeTrue();
        svc.DeleteItem("k3", 99).Should().BeFalse();
    }
}
