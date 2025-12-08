using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SenderService.API.Messages;

namespace SenderService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MessageController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost("product-updated")]
    public async Task<IActionResult> PublishProductUpdated([FromBody] ProductUpdatedMessage message)
    {
        await _publishEndpoint.Publish(message);
        return Ok(new { productId = message.ProductId });
    }
}

