using CartService.Application.DTOs;
using CartService.Domain.Entities;
using CartService.Domain.Repositories;

namespace CartService.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _repo;
    public CartService(ICartRepository repo)
    {
        _repo = repo;
    }

    public CartDto? GetCart(string key)
    {
        var cart = _repo.GetCart(key);
        if (cart == null) return null;
        return new CartDto { Key = cart.Key, Items = cart.Items.Select(i => new CartItemDto { Id = i.Id, ProductId = i.ProductId, Quantity = i.Quantity }).ToList() };
    }

    public IEnumerable<CartItemDto>? GetCartItems(string key)
    {
        var items = _repo.GetCartItems(key);
        return items?.Select(i => new CartItemDto { Id = i.Id, ProductId = i.ProductId, Quantity = i.Quantity });
    }

    public void AddItem(string key, AddCartItemDto item)
    {
        var cartItem = new CartItem { ProductId = item.ProductId, Quantity = item.Quantity };
        _repo.AddItem(key, cartItem);
    }

    public bool DeleteItem(string key, int itemId)
    {
        return _repo.DeleteItem(key, itemId);
    }

    public void UpdateCartItemsByProductId(int productId, string newName, decimal newPrice)
    {
        _repo.UpdateCartItemsByProductId(productId, newName, newPrice);
    }
}
