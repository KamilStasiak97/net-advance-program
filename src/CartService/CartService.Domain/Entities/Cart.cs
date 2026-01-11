// <copyright file="Cart.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CartService.Domain.Entities;

public class Cart
{
    public string Key { get; set; } = null!;

    public List<CartItem> Items { get; set; } = new();
}
