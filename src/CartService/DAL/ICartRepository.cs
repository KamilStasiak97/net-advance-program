using CartService.Domain.Models;
using System.Threading.Tasks;

namespace CartService.DAL
{
    public interface ICartRepository
    {
        Task<Cart> GetByIdAsync(string cartId);
        Task<Cart> CreateAsync(Cart cart);
        Task<Cart> UpdateAsync(Cart cart);
        Task<bool> DeleteAsync(string cartId);
        Task<bool> ExistsAsync(string cartId);
    }
}
