// <copyright file="AddCartItemDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CartService.API.DTOs;

public class AddCartItemDto
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }
}
