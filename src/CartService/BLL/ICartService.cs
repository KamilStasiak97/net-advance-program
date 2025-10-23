using CartService.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartService.BLL
{
    public interface ICartService
    {
        Task<Cart> GetCartAsync(string cartId);
        Task<Cart> CreateCartAsync(string cartId);
        Task<List<CartItem>> GetCartItemsAsync(string cartId);
        Task<Cart> AddItemAsync(string cartId, CartItem item);
        Task<Cart> RemoveItemAsync(string cartId, int itemId);
        Task<Cart> UpdateItemQuantityAsync(string cartId, int itemId, int quantity);
    }
}
