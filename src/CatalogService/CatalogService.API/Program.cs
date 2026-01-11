// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Reflection;
using CatalogService.API;
using CatalogService.API.Swagger;
using CatalogService.Domain.Entities;
using CatalogService.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // JWT Authentication
        var identityServerUrl = "http://localhost:5000";
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = identityServerUrl;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidIssuer = identityServerUrl,
                };
                options.RequireHttpsMetadata = false;
            });

        // Authorization
        builder.Services.AddAuthorization();

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
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header",
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        },
            });
        });

        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqHost = builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq";
                var rabbitMqUser = builder.Configuration["RabbitMQ:Username"] ?? "guest";
                var rabbitMqPass = builder.Configuration["RabbitMQ:Password"] ?? "guest";
                
                cfg.Host(rabbitMqHost, "/", h =>
                {
                    h.Username(rabbitMqUser);
                    h.Password(rabbitMqPass);
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
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        // Apply database migrations and seed data
        using (var scope = app.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            try
            {
                ctx.Database.EnsureCreated();
                if (!ctx.Categories.Any())
                {
                    var cat1 = new Category { Name = "Electronics", Description = "Electronic items" };
                    var cat2 = new Category { Name = "Books", Description = "Books and magazines" };
                    ctx.Categories.AddRange(cat1, cat2);
                    ctx.Products.AddRange(
                        new Product { Name = "Phone", Description = "Smartphone", Price = 699, Category = cat1 },
                        new Product { Name = "Laptop", Description = "Gaming laptop", Price = 1299, Category = cat1 },
                        new Product { Name = "Novel", Description = "Best-selling novel", Price = 19.99m, Category = cat2 });
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating or seeding the database.");
            }
        }

        app.Run();
    }
}

public partial class Program
{
}
