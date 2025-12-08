namespace CartService.Application.Messaging;

public record ProductUpdatedMessage
{
    public int ProductId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
}

