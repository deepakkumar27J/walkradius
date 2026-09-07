using Microsoft.Extensions.Caching.Memory;
using WalkInRadius.Domain.Entities;
using WalkInRadius.Domain.Interfaces;

namespace WalkInRadius.Infrastructure.RouteGeneration;

public class RouteGenerator : IRouteGenerator
{
    private readonly IWalkingDataService _walkingDataService;
    private readonly IMemoryCache _cache;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public RouteGenerator(IWalkingDataService walkingDataService, IMemoryCache cache)
    {
        _walkingDataService = walkingDataService;
        _cache = cache;
    }

    public async Task<Route> GenerateAsync(Walk walk)
    {
        var cacheKey = BuildCacheKey(walk);

        if (_cache.TryGetValue(cacheKey, out Route? cached) && cached is not null)
        {
            Console.WriteLine($"Cache HIT: {cacheKey}");
            return cached;
        }

        Console.WriteLine($"Cache MISS: {cacheKey}");

        var radiusKm = walk.Constraint.ToEstimatedDistanceKm();
        var effectiveRadiusKm = radiusKm / 2;

        var (coordinates, totalDistanceMetres) = await _walkingDataService
            .GetCircularRouteAsync(walk.StartPoint, effectiveRadiusKm);

        var totalDistanceKm = totalDistanceMetres / 1000;
        var route = new Route(walk.Id, coordinates.ToList(), totalDistanceKm);

        _cache.Set(cacheKey, route, CacheDuration);

        return route;
    }

    private static string BuildCacheKey(Walk walk)
    {
        // Round coordinates to 2 decimal places so nearby locations share a cache entry
        // Round constraint value to nearest 5 so similar durations share too
        var lat = Math.Round(walk.StartPoint.Latitude, 2);
        var lon = Math.Round(walk.StartPoint.Longitude, 2);
        var type = walk.Constraint.Type;
        var value = Math.Round(walk.Constraint.Value / 5) * 5;

        return $"route_{lat}_{lon}_{type}_{value}";
    }
}