using CartService.Domain.Models;
using LiteDB;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CartService.DAL
{
    public class LiteDbCartRepository : ICartRepository
    {
        private readonly string _connectionString;

        public LiteDbCartRepository(string connectionString = null)
        {
            _connectionString = connectionString ?? GetDefaultConnectionString();
            EnsureDirectoryExists();
        }

        private string GetDefaultConnectionString()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var dataDir = Path.Combine(baseDir, "Data");
            return Path.Combine(dataDir, "cart.db");
        }

        private void EnsureDirectoryExists()
        {
            var directory = Path.GetDirectoryName(_connectionString);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public Task<Cart> GetByIdAsync(string cartId)
        {
            using var db = new LiteDatabase(_connectionString);
            var carts = db.GetCollection<Cart>("carts");
            var cart = carts.FindById(cartId);
            return Task.FromResult(cart);
        }

        public Task<Cart> CreateAsync(Cart cart)
        {
            using var db = new LiteDatabase(_connectionString);
            var carts = db.GetCollection<Cart>("carts");
            cart.CreatedAt = DateTime.UtcNow;
            carts.Insert(cart.Id, cart);
            return Task.FromResult(cart);
        }

        public Task<Cart> UpdateAsync(Cart cart)
        {
            using var db = new LiteDatabase(_connectionString);
            var carts = db.GetCollection<Cart>("carts");
            cart.UpdatedAt = DateTime.UtcNow;
            carts.Update(cart.Id, cart);
            return Task.FromResult(cart);
        }

        public Task<bool> DeleteAsync(string cartId)
        {
            using var db = new LiteDatabase(_connectionString);
            var carts = db.GetCollection<Cart>("carts");
            return Task.FromResult(carts.Delete(cartId));
        }

        public Task<bool> ExistsAsync(string cartId)
        {
            using var db = new LiteDatabase(_connectionString);
            var carts = db.GetCollection<Cart>("carts");
            return Task.FromResult(carts.Exists(c => c.Id == cartId));
        }
    }
}
