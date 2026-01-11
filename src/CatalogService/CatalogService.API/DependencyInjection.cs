// <copyright file="DependencyInjection.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CatalogService.API;

using CatalogService.Application.Services;
using CatalogService.Domain.Repositories;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CatalogDb") 
            ?? "Host=catalog-db;Port=5432;Database=catalogdb;Username=catalog_user;Password=catalog_password";
        
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        // Application services
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
