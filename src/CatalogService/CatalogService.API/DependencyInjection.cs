using CatalogService.Application.Services;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.Repositories;
using CatalogService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.API;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseInMemoryDatabase("CatalogDb"));

    services.AddScoped<ICategoryRepository, CategoryRepository>();
    services.AddScoped<IProductRepository, ProductRepository>();

    // Application services
    services.AddScoped<ICategoryService, CategoryService>();
    services.AddScoped<IProductService, ProductService>();

        return services;
    }
}