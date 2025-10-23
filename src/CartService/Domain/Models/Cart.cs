using System;
using System.Collections.Generic;

namespace CartService.Domain.Models
{
    /// <summary>
    /// Represents a shopping cart with unique client-side generated ID
    /// </summary>
    public class Cart
    {
        /// <summary>
        /// Unique cart identifier maintained/generated on the client-side
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// List of items in the cart
        /// </summary>
        public List<CartItem> Items { get; set; } = new();

        /// <summary>
        /// Timestamp when the cart was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp when the cart was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        public Cart()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public Cart(string id) : this()
        {
            Id = id;
        }
    }
}
