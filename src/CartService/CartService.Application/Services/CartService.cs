// <copyright file="CartService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CartService.Application.Services;

using global::CartService.Application.DTOs;
using global::CartService.Domain.Entities;
using global::CartService.Domain.Repositories;

public class CartService : ICartService
{
    private readonly ICartRepository repo;

    public CartService(ICartRepository repo)
    {
        this.repo = repo;
    }

    public CartDto? GetCart(string key)
    {
        var cart = this.repo.GetCart(key);
        if (cart == null)
        {
            return null;
        }

        return new CartDto { Key = cart.Key, Items = cart.Items.Select(i => new CartItemDto { Id = i.Id, ProductId = i.ProductId, Quantity = i.Quantity }).ToList() };
    }

    public IEnumerable<CartItemDto>? GetCartItems(string key)
    {
        var items = this.repo.GetCartItems(key);
        return items?.Select(i => new CartItemDto { Id = i.Id, ProductId = i.ProductId, Quantity = i.Quantity });
    }

    public void AddItem(string key, AddCartItemDto item)
    {
        var cartItem = new CartItem { ProductId = item.ProductId, Quantity = item.Quantity };
        this.repo.AddItem(key, cartItem);
    }

    public bool DeleteItem(string key, int itemId)
    {
        return this.repo.DeleteItem(key, itemId);
    }

    public void UpdateCartItemsByProductId(int productId, string newName, decimal newPrice)
    {
        this.repo.UpdateCartItemsByProductId(productId, newName, newPrice);
    }
}
