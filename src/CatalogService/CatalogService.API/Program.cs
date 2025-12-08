using Microsoft.AspNetCore.Builder;
using CatalogService.API;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CatalogService.Infrastructure.Data;
using CatalogService.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using System.Reflection;
using CatalogService.API.Swagger;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// API versioning + explorer
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // e.g. v1
    options.SubstituteApiVersionInUrl = true;
});

// Swagger
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Configure endpoints with message durability and republish on error
        cfg.ConfigureEndpoints(context);

        // Ensure all published messages are durable (persisted to disk)
        cfg.Publish<CartService.Application.Messaging.ProductUpdatedMessage>(p =>
        {
            p.Durable = true;
        });
    });
});

builder.Services.AddCatalogServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Seed some sample data on startup
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    if (!ctx.Categories.Any())
    {
        var cat1 = new Category { Name = "Electronics", Description = "Electronic items" };
        var cat2 = new Category { Name = "Books", Description = "Books and magazines" };
        ctx.Categories.AddRange(cat1, cat2);
        ctx.Products.AddRange(
            new Product { Name = "Phone", Description = "Smartphone", Price = 699, Category = cat1 },
            new Product { Name = "Laptop", Description = "Gaming laptop", Price = 1299, Category = cat1 },
            new Product { Name = "Novel", Description = "Best-selling novel", Price = 19.99m, Category = cat2 }
        );
        ctx.SaveChanges();
    }
}

app.Run();

public partial class Program { }
