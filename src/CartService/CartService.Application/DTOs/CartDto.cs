namespace CartService.Application.DTOs;

public class CartDto
{
    public string Key { get; set; } = null!;
    public List<CartItemDto> Items { get; set; } = new();
}
