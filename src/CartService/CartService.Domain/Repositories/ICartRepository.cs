// <copyright file="ICartRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CartService.Domain.Repositories;

using CartService.Domain.Entities;

public interface ICartRepository
{
    Cart? GetCart(string key);

    IEnumerable<CartItem>? GetCartItems(string key);

    void AddItem(string key, CartItem item);

    bool DeleteItem(string key, int itemId);

    void UpdateCartItemsByProductId(int productId, string newName, decimal newPrice);
}
