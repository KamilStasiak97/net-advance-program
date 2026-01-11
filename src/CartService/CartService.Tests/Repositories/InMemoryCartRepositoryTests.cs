// <copyright file="InMemoryCartRepositoryTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CartService.Tests.Repositories;

using System.Linq;
using CartService.Domain.Entities;
using CartService.Infrastructure.Repositories;
using FluentAssertions;
using Xunit;

public class InMemoryCartRepositoryTests
{
    [Fact]
    public void AddAndGetItems_PersistsPerCartKey()
    {
        var repo = new InMemoryCartRepository();

        repo.AddItem("a", new CartItem { ProductId = 1, Quantity = 1 });
        repo.AddItem("a", new CartItem { ProductId = 2, Quantity = 2 });
        repo.AddItem("b", new CartItem { ProductId = 9, Quantity = 9 });

        var itemsA = repo.GetCartItems("a")?.ToList() ?? new List<CartItem>();
        var itemsB = repo.GetCartItems("b")?.ToList() ?? new List<CartItem>();

        itemsA.Should().HaveCount(2);
        itemsB.Should().HaveCount(1);
        itemsA.Select(i => i.ProductId).Should().Contain(new[] { 1, 2 });
    }

    [Fact]
    public void DeleteItem_RemovesItem()
    {
        var repo = new InMemoryCartRepository();
        repo.AddItem("c", new CartItem { ProductId = 3, Quantity = 1 });
        var items = repo.GetCartItems("c")?.ToList() ?? new List<CartItem>();
        var id = items[0].Id;

        var removed = repo.DeleteItem("c", id);
        removed.Should().BeTrue();
        repo.GetCartItems("c").Should().BeNullOrEmpty();
    }
}
