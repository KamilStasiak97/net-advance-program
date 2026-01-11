using CartService.Domain.Entities;
using CartService.Domain.Repositories;

namespace CartService.Infrastructure.Repositories;

public class InMemoryCartRepository : ICartRepository
{
    private readonly Dictionary<string, List<CartItem>> _store = new();

    public Cart? GetCart(string key)
    {
        if (!_store.ContainsKey(key)) return null;
        return new Cart { Key = key, Items = _store[key].Select(i => new CartItem { Id = i.Id, ProductId = i.ProductId, Quantity = i.Quantity }).ToList() };
    }

    public IEnumerable<CartItem>? GetCartItems(string key)
    {
        if (!_store.ContainsKey(key)) return null;
        return _store[key].Select(i => new CartItem { Id = i.Id, ProductId = i.ProductId, Quantity = i.Quantity });
    }

    public void AddItem(string key, CartItem item)
    {
        if (!_store.ContainsKey(key)) _store[key] = new List<CartItem>();
        var list = _store[key];
        var nextId = list.Any() ? list.Max(i => i.Id) + 1 : 1;
        item.Id = nextId;
        list.Add(item);
    }

    public bool DeleteItem(string key, int itemId)
    {
        if (!_store.ContainsKey(key)) return false;
        var list = _store[key];
        var item = list.FirstOrDefault(i => i.Id == itemId);
        if (item == null) return false;
        list.Remove(item);
        return true;
    }

    public void UpdateCartItemsByProductId(int productId, string newName, decimal newPrice)
    {
        foreach (var cartItems in _store.Values)
        {
            var itemsToUpdate = cartItems.Where(i => i.ProductId == productId).ToList();
            foreach (var item in itemsToUpdate)
            {
                // Update cart item properties when product is updated
                // Note: CartItem doesn't store name/price, but this method signature
                // is required by the interface. In a real scenario, you might want
                // to store product name/price in CartItem or fetch from product service.
            }
        }
    }
}
