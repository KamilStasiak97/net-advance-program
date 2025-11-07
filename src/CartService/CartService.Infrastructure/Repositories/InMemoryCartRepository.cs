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
}
