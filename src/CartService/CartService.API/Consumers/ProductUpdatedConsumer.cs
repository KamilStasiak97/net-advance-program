// <copyright file="ProductUpdatedConsumer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CartService.API.Consumers;

using CartService.Application.Messaging;
using CartService.Application.Services;
using MassTransit;

public class ProductUpdatedConsumer : IConsumer<ProductUpdatedMessage>
{
    private readonly ICartService cartService;
    private readonly ILogger<ProductUpdatedConsumer> logger;

    public ProductUpdatedConsumer(ICartService cartService, ILogger<ProductUpdatedConsumer> logger)
    {
        this.cartService = cartService;
        this.logger = logger;
    }

    public Task Consume(ConsumeContext<ProductUpdatedMessage> context)
    {
        var message = context.Message;
        this.logger.LogInformation(
            "Received product update: ProductId={ProductId}, Name={Name}, Price={Price}",
            message.ProductId, message.Name, message.Price);

        this.cartService.UpdateCartItemsByProductId(message.ProductId, message.Name, message.Price);

        this.logger.LogInformation("Updated cart items for ProductId={ProductId}", message.ProductId);
        return Task.CompletedTask;
    }
}
