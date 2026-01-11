// <copyright file="CartDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CartService.Application.DTOs;

public class CartDto
{
    public string Key { get; set; } = null!;

    public List<CartItemDto> Items { get; set; } = new();
}
