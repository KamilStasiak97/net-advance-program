// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using CartService.API.Consumers;
using CartService.API.Middleware;
using CartService.API.Swagger;
using CartService.Infrastructure;
using CartServiceApp = CartService.Application.Services;
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

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

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
            x.AddConsumer<ProductUpdatedConsumer>();

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

        builder.Services.AddCartInfrastructure(builder.Configuration);
        builder.Services.AddScoped<CartServiceApp.ICartService, CartServiceApp.CartService>();

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

        // Add token logging middleware
        app.UseMiddleware<TokenLoggingMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        // Apply database migrations
        using (var scope = app.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<CartService.Infrastructure.Data.CartDbContext>();
            try
            {
                ctx.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating the database.");
            }
        }

        app.Run();
    }
}

namespace CartService.API
{
    public partial class Program
    {
    }
}
