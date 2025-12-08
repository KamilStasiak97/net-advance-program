using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using CartService.API.Swagger;
using CartService.Infrastructure;
using CartService.Application.Services;
using MassTransit;
using CartService.API.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductUpdatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Exponential retry policy: 3 retries with exponential backoff
        cfg.UseMessageRetry(r => r.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2)));

        // Configure consumer endpoint with durability for guaranteed delivery
        cfg.ReceiveEndpoint("product-updated-queue", e =>
        {
            // Durable queue - survives broker restart
            e.Durable = true;
            e.AutoDelete = false;

            // Configure consumer
            e.ConfigureConsumer<ProductUpdatedConsumer>(context);
        });
    });
});

builder.Services.AddCartInfrastructure();
builder.Services.AddScoped<CartService.Application.Services.ICartService, CartService.Application.Services.CartService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
        }
    });
}

app.MapControllers();

app.Run();

namespace CartService.API
{
    public partial class Program { }
}
