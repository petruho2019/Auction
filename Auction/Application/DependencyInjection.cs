using AuctionApi.Application.Interfaces;
using AuctionApi.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuctionService, AuctionService>();
        return services;
    }
}
