// <copyright file="CartItemDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CartService.API.DTOs;

public class CartItemDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }
}
