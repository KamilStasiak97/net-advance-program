// <copyright file="ICartService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CartService.Application.Services;

using global::CartService.Application.DTOs;

public interface ICartService
{
    CartDto? GetCart(string key);

    IEnumerable<CartItemDto>? GetCartItems(string key);

    void AddItem(string key, AddCartItemDto item);

    bool DeleteItem(string key, int itemId);

    void UpdateCartItemsByProductId(int productId, string newName, decimal newPrice);
}
