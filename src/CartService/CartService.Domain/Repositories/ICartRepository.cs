using CartService.Domain.Entities;

namespace CartService.Domain.Repositories;

public interface ICartRepository
{
    Cart? GetCart(string key);
    IEnumerable<CartItem>? GetCartItems(string key);
    void AddItem(string key, CartItem item);
    bool DeleteItem(string key, int itemId);
    void UpdateCartItemsByProductId(int productId, string newName, decimal newPrice);
}
