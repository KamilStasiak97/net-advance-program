// <copyright file="DependencyInjection.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CartService.Infrastructure;

using CartService.Domain.Repositories;
using CartService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddCartInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CartDb") 
            ?? "Host=cart-db;Port=5432;Database=cartdb;Username=cart_user;Password=cart_password";
        
        services.AddDbContext<Data.CartDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddScoped<ICartRepository, Repositories.PostgresCartRepository>();
        return services;
    }
}
