// <copyright file="InMemoryCartRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CartService.Infrastructure.Repositories;

using CartService.Domain.Entities;
using CartService.Domain.Repositories;

public class InMemoryCartRepository : ICartRepository
{
    private readonly Dictionary<string, List<CartItem>> store = new();

    public Cart? GetCart(string key)
    {
        if (!this.store.ContainsKey(key))
        {
            return null;
        }

        return new Cart { Key = key, Items = this.store[key].Select(i => new CartItem { Id = i.Id, ProductId = i.ProductId, Quantity = i.Quantity }).ToList() };
    }

    public IEnumerable<CartItem>? GetCartItems(string key)
    {
        if (!this.store.ContainsKey(key))
        {
            return null;
        }

        return this.store[key].Select(i => new CartItem { Id = i.Id, ProductId = i.ProductId, Quantity = i.Quantity });
    }

    public void AddItem(string key, CartItem item)
    {
        if (!this.store.ContainsKey(key))
        {
            this.store[key] = new List<CartItem>();
        }

        var list = this.store[key];
        var nextId = list.Any() ? list.Max(i => i.Id) + 1 : 1;
        item.Id = nextId;
        list.Add(item);
    }

    public bool DeleteItem(string key, int itemId)
    {
        if (!this.store.ContainsKey(key))
        {
            return false;
        }

        var list = this.store[key];
        var item = list.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            return false;
        }

        list.Remove(item);
        return true;
    }

    public void UpdateCartItemsByProductId(int productId, string newName, decimal newPrice)
    {
        foreach (var cartItems in this.store.Values)
        {
            var itemsToUpdate = cartItems.Where(i => i.ProductId == productId).ToList();
            foreach (var item in itemsToUpdate)
            {
                // Update cart item properties when product is updated
                // Note: CartItem doesn't store name/price, but this method signature
                // is required by the interface. In a real scenario, you might want
                // to store product name/price in CartItem or fetch from product service.
            }
        }
    }
}
