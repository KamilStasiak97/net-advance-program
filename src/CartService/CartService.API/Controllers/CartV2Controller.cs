// <copyright file="CartV2Controller.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CartService.API.Controllers;

using CartService.Application.DTOs;
using CartService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{version:apiVersion}/Cart")]
[ApiVersion("2.0")]
[Authorize(Roles = "Manager,Store customer")]
public class CartV2Controller : ControllerBase
{
    private readonly ICartService cartService;

    public CartV2Controller(ICartService cartService)
    {
        this.cartService = cartService;
    }

    /// <summary>
    /// Gets cart items by key. Returns only list of items (not cart model).
    /// </summary>
    /// <param name="key">Cart unique key.</param>
    /// <returns>List of cart items.</returns>
    [HttpGet("{key}")]
    [ProducesResponseType(typeof(IEnumerable<CartItemDto>), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetItems(string key)
    {
        var items = this.cartService.GetCartItems(key);
        if (items == null)
        {
            return this.NotFound();
        }

        return this.Ok(items);
    }

    /// <summary>
    /// Adds item to cart. Creates cart if it doesn't exist.
    /// </summary>
    /// <param name="key">Cart unique key.</param>
    /// <param name="item">Cart item to add.</param>
    /// <returns></returns>
    [HttpPost("{key}/items")]
    [ProducesResponseType(200)]
    public IActionResult AddItem(string key, [FromBody] AddCartItemDto item)
    {
        this.cartService.AddItem(key, item);
        return this.Ok();
    }

    /// <summary>
    /// Deletes item from cart.
    /// </summary>
    /// <param name="key">Cart unique key.</param>
    /// <param name="itemId">Item ID to delete.</param>
    /// <returns></returns>
    [HttpDelete("{key}/items/{itemId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public IActionResult DeleteItem(string key, int itemId)
    {
        var removed = this.cartService.DeleteItem(key, itemId);
        if (!removed)
        {
            return this.NotFound();
        }

        return this.Ok();
    }
}
