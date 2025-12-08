using MassTransit;
using CartService.Application.Messaging;
using CartService.Application.Services;

namespace CartService.API.Consumers;

public class ProductUpdatedConsumer : IConsumer<ProductUpdatedMessage>
{
    private readonly ICartService _cartService;
    private readonly ILogger<ProductUpdatedConsumer> _logger;

    public ProductUpdatedConsumer(ICartService cartService, ILogger<ProductUpdatedConsumer> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    public Task Consume(ConsumeContext<ProductUpdatedMessage> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received product update: ProductId={ProductId}, Name={Name}, Price={Price}", 
            message.ProductId, message.Name, message.Price);

        _cartService.UpdateCartItemsByProductId(message.ProductId, message.Name, message.Price);
        
        _logger.LogInformation("Updated cart items for ProductId={ProductId}", message.ProductId);
        return Task.CompletedTask;
    }
}

