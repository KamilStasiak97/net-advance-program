using Microsoft.AspNetCore.Mvc;
using CartService.Application.Services;
using CartService.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace CartService.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/Cart")]
[ApiVersion("1.0")]
[Authorize(Roles = "Manager,Store customer")]
public class CartV1Controller : ControllerBase
{
    private readonly ICartService _cartService;
    
    public CartV1Controller(ICartService cartService)
    {
        _cartService = cartService;
    }

    /// <summary>
    /// Gets cart by key. Returns cart model with key and items.
    /// </summary>
    /// <param name="key">Cart unique key</param>
    /// <returns>Cart model with key and list of cart items</returns>
    [HttpGet("{key}")]
    [ProducesResponseType(typeof(CartDto), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetCart(string key)
    {
        var cart = _cartService.GetCart(key);
        if (cart == null) return NotFound();
        return Ok(cart);
    }

    /// <summary>
    /// Adds item to cart. Creates cart if it doesn't exist.
    /// </summary>
    /// <param name="key">Cart unique key</param>
    /// <param name="item">Cart item to add</param>
    [HttpPost("{key}/items")]
    [ProducesResponseType(200)]
    public IActionResult AddItem(string key, [FromBody] AddCartItemDto item)
    {
        _cartService.AddItem(key, item);
        return Ok();
    }

    /// <summary>
    /// Deletes item from cart.
    /// </summary>
    /// <param name="key">Cart unique key</param>
    /// <param name="itemId">Item ID to delete</param>
    [HttpDelete("{key}/items/{itemId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public IActionResult DeleteItem(string key, int itemId)
    {
        var removed = _cartService.DeleteItem(key, itemId);
        if (!removed) return NotFound();
        return Ok();
    }
}
