using CartService.Application.DTOs;

namespace CartService.Application.Services;

public interface ICartService
{
    CartDto? GetCart(string key);
    IEnumerable<CartItemDto>? GetCartItems(string key);
    void AddItem(string key, AddCartItemDto item);
    bool DeleteItem(string key, int itemId);
}
