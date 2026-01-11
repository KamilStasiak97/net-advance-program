// <copyright file="CartServiceUnitTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CartService.Tests.Services;

using CartService.Application.DTOs;
using CartService.Application.Services;
using CartService.Infrastructure.Repositories;
using FluentAssertions;
using Xunit;

public class CartServiceUnitTests
{
    [Fact]
    public void AddItem_GetCart_DeleteItem_Workflow()
    {
        var repo = new InMemoryCartRepository();
        var svc = new CartService.Application.Services.CartService(repo);
        var key = "cart-1";

        svc.AddItem(key, new AddCartItemDto { ProductId = 5, Quantity = 2 });
        var cart = svc.GetCart(key);
        cart.Should().NotBeNull();
        cart!.Items.Should().HaveCount(1);
        cart.Items[0].ProductId.Should().Be(5);

        svc.AddItem(key, new AddCartItemDto { ProductId = 6, Quantity = 1 });
        var items = svc.GetCartItems(key)?.ToList();
        items.Should().HaveCount(2);

        var firstId = items![0].Id;
        var removed = svc.DeleteItem(key, firstId);
        removed.Should().BeTrue();
        svc.GetCartItems(key)!.Should().HaveCount(1);
    }
}
