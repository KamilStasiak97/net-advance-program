namespace CartService.Domain.Models
{
    /// <summary>
    /// Represents an item in the shopping cart
    /// </summary>
    public class CartItem
    {
        /// <summary>
        /// Required. Id of the item in external system
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Required. Plain text name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Optional. Image with URL and alt text
        /// </summary>
        public Image? Image { get; set; }

        /// <summary>
        /// Required. Price in money
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Quantity of items in the cart
        /// </summary>
        public int Quantity { get; set; }

        public CartItem()
        {
        }

        public CartItem(int id, string name, decimal price, int quantity, Image? image = null)
        {
            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
            Image = image;
        }
    }
}
