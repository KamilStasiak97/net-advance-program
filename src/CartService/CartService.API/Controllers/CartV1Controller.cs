// <copyright file="CartV1Controller.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CartService.API.Controllers;

using CartService.Application.DTOs;
using CartService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{version:apiVersion}/Cart")]
[ApiVersion("1.0")]
[Authorize(Roles = "Manager,Store customer")]
public class CartV1Controller : ControllerBase
{
    private readonly ICartService cartService;

    public CartV1Controller(ICartService cartService)
    {
        this.cartService = cartService;
    }

    /// <summary>
    /// Gets cart by key. Returns cart model with key and items.
    /// </summary>
    /// <param name="key">Cart unique key.</param>
    /// <returns>Cart model with key and list of cart items.</returns>
    [HttpGet("{key}")]
    [ProducesResponseType(typeof(CartDto), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetCart(string key)
    {
        var cart = this.cartService.GetCart(key);
        if (cart == null)
        {
            return this.NotFound();
        }

        return this.Ok(cart);
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
