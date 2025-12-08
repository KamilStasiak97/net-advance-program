namespace CartService.Domain.Entities;

public class Cart
{
    public string Key { get; set; } = null!;
    public List<CartItem> Items { get; set; } = new();
}
