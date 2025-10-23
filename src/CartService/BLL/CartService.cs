using CartService.DAL;
using CartService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CartService.BLL
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repository;

        public CartService(ICartRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Cart> GetCartAsync(string cartId)
        {
            ValidateCartId(cartId);
            return await _repository.GetByIdAsync(cartId);
        }

        public async Task<Cart> CreateCartAsync(string cartId)
        {
            ValidateCartId(cartId);

            if (await _repository.ExistsAsync(cartId))
            {
                throw new InvalidOperationException($"Cart with ID '{cartId}' already exists.");
            }

            var cart = new Cart { Id = cartId };
            return await _repository.CreateAsync(cart);
        }

        public async Task<List<CartItem>> GetCartItemsAsync(string cartId)
        {
            var cart = await GetCartAsync(cartId);
            return cart?.Items ?? new List<CartItem>();
        }

        public async Task<Cart> AddItemAsync(string cartId, CartItem item)
        {
            ValidateCartId(cartId);
            ValidateItem(item);

            var cart = await _repository.GetByIdAsync(cartId);
            if (cart == null)
            {
                cart = new Cart { Id = cartId };
                await _repository.CreateAsync(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.Id == item.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Items.Add(item);
            }

            return await _repository.UpdateAsync(cart);
        }

        public async Task<Cart> RemoveItemAsync(string cartId, int itemId)
        {
            ValidateCartId(cartId);

            var cart = await _repository.GetByIdAsync(cartId);
            if (cart == null)
            {
                throw new InvalidOperationException($"Cart with ID '{cartId}' not found.");
            }

            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                cart.Items.Remove(item);
                return await _repository.UpdateAsync(cart);
            }

            return cart;
        }

        public async Task<Cart> UpdateItemQuantityAsync(string cartId, int itemId, int quantity)
        {
            ValidateCartId(cartId);

            if (quantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));
            }

            var cart = await _repository.GetByIdAsync(cartId);
            if (cart == null)
            {
                throw new InvalidOperationException($"Cart with ID '{cartId}' not found.");
            }

            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
            {
                throw new InvalidOperationException($"Item with ID '{itemId}' not found in cart.");
            }

            if (quantity == 0)
            {
                cart.Items.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }

            return await _repository.UpdateAsync(cart);
        }

        private void ValidateCartId(string cartId)
        {
            if (string.IsNullOrWhiteSpace(cartId))
            {
                throw new ArgumentException("Cart ID cannot be empty.", nameof(cartId));
            }
        }

        private void ValidateItem(CartItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.Id <= 0)
            {
                throw new ArgumentException("Item ID must be positive.", nameof(item.Id));
            }

            if (string.IsNullOrWhiteSpace(item.Name))
            {
                throw new ArgumentException("Item name is required.", nameof(item.Name));
            }

            if (item.Price < 0)
            {
                throw new ArgumentException("Item price cannot be negative.", nameof(item.Price));
            }

            if (item.Quantity <= 0)
            {
                throw new ArgumentException("Item quantity must be positive.", nameof(item.Quantity));
            }
        }
    }
}
