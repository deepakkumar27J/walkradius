using WalkInRadius.Domain.Interfaces;
using WalkInRadius.Domain.ValueObjects;
using WalkInRadius.Infrastructure.ExternalApis.FourSquarePlaces;
using WalkInRadius.Infrastructure.ExternalApis.OpenRouteService;

namespace WalkInRadius.Infrastructure.Services;

public class WalkingDataService : IWalkingDataService
{
    private readonly OpenRouteServiceClient _orsClient;

    public WalkingDataService(OpenRouteServiceClient orsClient)
    {
        _orsClient = orsClient;
    }

    public async Task<(IEnumerable<Coordinate> Waypoints, double TotalDistanceMetres)> GetCircularRouteAsync(
    Coordinate start, double radiusKm)
    {
        var rangeSeconds = (radiusKm / 5.0) * 3600;

        var isochrone = await _orsClient.GetIsochroneAsync(
            start.Latitude, start.Longitude, rangeSeconds);

        if (isochrone?.Features is null || isochrone.Features.Count == 0)
            throw new InvalidOperationException("Could not get walkable area from routing service.");

        var polygon = isochrone.Features[0].Geometry.Coordinates[0];
        var waypoints = PickWaypoints(start, polygon);

        var directions = await _orsClient.GetDirectionsAsync(waypoints);

        if (directions?.Features is null || directions.Features.Count == 0)
            throw new InvalidOperationException("Could not generate walking route.");

        var feature = directions.Features[0];
        var totalDistanceMetres = feature.Properties.Summary.Distance;

        var coordinates = feature.Geometry.Coordinates
            .Select(c => new Coordinate(c[1], c[0]))
            .ToList();

        return (coordinates, totalDistanceMetres);
    }
    // Pick evenly spaced waypoints

    private static IEnumerable<(double Latitude, double Longitude)> PickWaypoints(
        Coordinate start,
        List<List<double>> polygon)
    {
        // Start with user location
        var points = new List<(double Lat, double Lon)>
        {
            (start.Latitude, start.Longitude)
        };

        // Pick 3 evenly spaced points from the polygon boundry, creating a circular path
        var step = polygon.Count / 3;
        for (int i = 0; i<3; i++)
        {
            var point = polygon[i * step];
            points.Add((point[1], point[0]));
        }

        //return to start to close the loop
        points.Add((start.Latitude, start.Longitude));

        return points;
    }

    private static double CalculateBearing(
    double startLat, double startLon,
    double pointLat, double pointLon)
    {
        // Convert to radians
        var lat1 = startLat * Math.PI / 180;
        var lat2 = pointLat * Math.PI / 180;
        var deltaLon = (pointLon - startLon) * Math.PI / 180;

        var x = Math.Sin(deltaLon) * Math.Cos(lat2);
        var y = Math.Cos(lat1) * Math.Sin(lat2)
              - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(deltaLon);

        var bearing = Math.Atan2(x, y) * 180 / Math.PI;

        // Normalise to 0-360
        return (bearing + 360) % 360;
    }

    private static double CalculateDistanceKm(
    double lat1, double lon1,
    double lat2, double lon2)
    {
        const double R = 6371; // Earth radius in km
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(lat1 * Math.PI / 180)
              * Math.Cos(lat2 * Math.PI / 180)
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

}
