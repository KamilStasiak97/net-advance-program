// <copyright file="CartItem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CartService.Domain.Entities;

public class CartItem
{
    public int Id { get; set; }

    public string CartKey { get; set; } = null!;

    public int ProductId { get; set; }

    public int Quantity { get; set; }
}
