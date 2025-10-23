using CartService.BLL;
using CartService.DAL;
using CartService.Domain.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CartService.Tests
{
    public class CartServiceTests
    {
        private ICartService CreateService()
        {
            var repository = new InMemoryCartRepository();
            return new BLL.CartService(repository);
        }

        [Fact]
        public async Task AddItem_ShouldAddNewItemToCart()
        {
            var service = CreateService();
            var cartId = "test-cart-123";

            var item = new CartItem
            {
                Id = 1,
                Name = "Test Product",
                Price = 99.99m,
                Quantity = 2
            };

            var cart = await service.AddItemAsync(cartId, item);

            Assert.NotNull(cart);
            Assert.Single(cart.Items);
            Assert.Equal(1, cart.Items[0].Id);
            Assert.Equal(2, cart.Items[0].Quantity);
        }

        [Fact]
        public async Task AddItem_ShouldIncreaseQuantityForExistingItem()
        {
            var service = CreateService();
            var cartId = "test-cart-456";

            var item1 = new CartItem { Id = 1, Name = "Product", Price = 50m, Quantity = 1 };
            var item2 = new CartItem { Id = 1, Name = "Product", Price = 50m, Quantity = 2 };

            await service.AddItemAsync(cartId, item1);
            var cart = await service.AddItemAsync(cartId, item2);

            Assert.Single(cart.Items);
            Assert.Equal(3, cart.Items[0].Quantity);
        }

        [Fact]
        public async Task RemoveItem_ShouldRemoveItemFromCart()
        {
            var service = CreateService();
            var cartId = "test-cart-789";

            var item = new CartItem { Id = 1, Name = "Product", Price = 50m, Quantity = 1 };
            await service.AddItemAsync(cartId, item);

            var cart = await service.RemoveItemAsync(cartId, 1);

            Assert.Empty(cart.Items);
        }

        [Fact]
        public async Task AddItem_WithInvalidData_ShouldThrowException()
        {
            var service = CreateService();
            var cartId = "test-cart";

            var invalidItem = new CartItem { Id = -1, Name = "", Price = -10m, Quantity = 0 };

            await Assert.ThrowsAsync<ArgumentException>(() => service.AddItemAsync(cartId, invalidItem));
        }

        [Fact]
        public async Task GetCartItems_ShouldReturnAllItems()
        {
            var service = CreateService();
            var cartId = "test-cart-multi";

            await service.AddItemAsync(cartId, new CartItem { Id = 1, Name = "Item 1", Price = 10m, Quantity = 1 });
            await service.AddItemAsync(cartId, new CartItem { Id = 2, Name = "Item 2", Price = 20m, Quantity = 2 });

            var items = await service.GetCartItemsAsync(cartId);

            Assert.Equal(2, items.Count);
        }
    }

    public class InMemoryCartRepository : ICartRepository
    {
        private readonly System.Collections.Generic.Dictionary<string, Cart> _carts = new();

        public Task<Cart> GetByIdAsync(string cartId)
        {
            _carts.TryGetValue(cartId, out var cart);
            return Task.FromResult(cart);
        }

        public Task<Cart> CreateAsync(Cart cart)
        {
            _carts[cart.Id] = cart;
            return Task.FromResult(cart);
        }

        public Task<Cart> UpdateAsync(Cart cart)
        {
            cart.UpdatedAt = DateTime.UtcNow;
            _carts[cart.Id] = cart;
            return Task.FromResult(cart);
        }

        public Task<bool> DeleteAsync(string cartId)
        {
            return Task.FromResult(_carts.Remove(cartId));
        }

        public Task<bool> ExistsAsync(string cartId)
        {
            return Task.FromResult(_carts.ContainsKey(cartId));
        }
    }
}
