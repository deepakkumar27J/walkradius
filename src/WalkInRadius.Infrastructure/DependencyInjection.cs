using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WalkInRadius.Domain.Interfaces;
using WalkInRadius.Infrastructure.ExternalApis.FourSquarePlaces;
using WalkInRadius.Infrastructure.ExternalApis.OpenRouteService;
using WalkInRadius.Infrastructure.RouteGeneration;
using WalkInRadius.Infrastructure.Services;

namespace WalkInRadius.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var apiKey = configuration["OpenRouteServices:ApiKey"]
            ?? throw new InvalidOperationException("ORS API Key is not configured.");

        //register httpClient for ors with base URL and API Key header
        services.AddHttpClient<OpenRouteServiceClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.openrouteservice.org/");
            client.DefaultRequestHeaders.Add("Authorization", apiKey);
        });

        services.AddMemoryCache();

        // blind interfaces to implementation
        services.AddScoped<IWalkingDataService, WalkingDataService>();
        services.AddScoped<IRouteGenerator, RouteGenerator>();

        return services;
    }
}
