using MassTransit;
using ReceiverService.API.Messages;

namespace ReceiverService.API.Consumers;

public class ProductUpdatedConsumer : IConsumer<ProductUpdatedMessage>
{
    private readonly ILogger<ProductUpdatedConsumer> _logger;

    public ProductUpdatedConsumer(ILogger<ProductUpdatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductUpdatedMessage> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received message: ProductId={ProductId}, Name={Name}, Price={Price}", 
            message.ProductId, message.Name, message.Price);
        
        await Task.Delay(100);
        
        _logger.LogInformation("Processed ProductId={ProductId}", message.ProductId);
    }
}

