using Microsoft.Extensions.DependencyInjection;
using CartService.Domain.Repositories;
using CartService.Infrastructure.Repositories;

namespace CartService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCartInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ICartRepository, InMemoryCartRepository>();
        return services;
    }
}
