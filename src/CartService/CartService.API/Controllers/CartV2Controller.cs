using Microsoft.AspNetCore.Mvc;
using CartService.Application.Services;
using CartService.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace CartService.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/Cart")]
[ApiVersion("2.0")]
[Authorize(Roles = "Manager,Store customer")]
public class CartV2Controller : ControllerBase
{
    private readonly ICartService _cartService;
    
    public CartV2Controller(ICartService cartService)
    {
        _cartService = cartService;
    }

    /// <summary>
    /// Gets cart items by key. Returns only list of items (not cart model).
    /// </summary>
    /// <param name="key">Cart unique key</param>
    /// <returns>List of cart items</returns>
    [HttpGet("{key}")]
    [ProducesResponseType(typeof(IEnumerable<CartItemDto>), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetItems(string key)
    {
        var items = _cartService.GetCartItems(key);
        if (items == null) return NotFound();
        return Ok(items);
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
