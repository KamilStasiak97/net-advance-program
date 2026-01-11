namespace CartService.Infrastructure.Repositories;

using CartService.Domain.Entities;
using CartService.Domain.Repositories;
using CartService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class PostgresCartRepository : ICartRepository
{
    private readonly CartDbContext _context;

    public PostgresCartRepository(CartDbContext context)
    {
        _context = context;
    }

    public Cart? GetCart(string key)
    {
        return _context.Carts
            .Include(c => c.Items)
            .FirstOrDefault(c => c.Key == key);
    }

    public IEnumerable<CartItem>? GetCartItems(string key)
    {
        var cart = GetCart(key);
        return cart?.Items;
    }

    public void AddItem(string key, CartItem item)
    {
        var cart = _context.Carts
            .Include(c => c.Items)
            .FirstOrDefault(c => c.Key == key);

        if (cart == null)
        {
            cart = new Cart { Key = key, Items = new List<CartItem>() };
            _context.Carts.Add(cart);
        }

        item.CartKey = key;
        if (cart.Items.Any())
        {
            item.Id = cart.Items.Max(i => i.Id) + 1;
        }
        else
        {
            item.Id = 1;
        }

        cart.Items.Add(item);
        _context.SaveChanges();
    }

    public bool DeleteItem(string key, int itemId)
    {
        var cart = _context.Carts
            .Include(c => c.Items)
            .FirstOrDefault(c => c.Key == key);

        if (cart == null)
        {
            return false;
        }

        var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            return false;
        }

        cart.Items.Remove(item);
        _context.SaveChanges();
        return true;
    }

    public void UpdateCartItemsByProductId(int productId, string newName, decimal newPrice)
    {
        // Note: CartItem doesn't store name/price, but this method signature
        // is required by the interface. In a real scenario, you might want
        // to store product name/price in CartItem or fetch from product service.
        var items = _context.CartItems
            .Where(i => i.ProductId == productId)
            .ToList();

        // No actual update needed as CartItem doesn't store name/price
        _context.SaveChanges();
    }
}

