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
    List<List<double>> polygon,
    int numberOfWaypoints = 3,
    double minDistanceRatio = 0.30)
    {
        // Step 1 — calculate bearing and distance for every polygon point
        var pointsWithData = polygon.Select(point => new
        {
            Lon = point[0],
            Lat = point[1],
            Distance = CalculateDistanceKm(
                start.Latitude, start.Longitude,
                point[1], point[0]),
            Bearing = CalculateBearing(
                start.Latitude, start.Longitude,
                point[1], point[0])
        }).ToList();

        // Step 2 — filter out points too close to start
        var maxDistance = pointsWithData.Max(p => p.Distance);
        var minDistance = maxDistance * minDistanceRatio;

        var filteredPoints = pointsWithData
            .Where(p => p.Distance >= minDistance)
            .ToList();

        // Step 3 — pick one point per target angle, evenly spread around 360°
        var result = new List<(double Lat, double Lon)>
        {
            (start.Latitude, start.Longitude)
        };

        for (int i = 0; i < numberOfWaypoints; i++)
        {
            var targetBearing = (360.0 / numberOfWaypoints) * i;

            // pick closest by bearing, prefer further distance, must be distinct
            var best = filteredPoints
                .OrderBy(p => AngleDifference(p.Bearing, targetBearing))
                .ThenByDescending(p => p.Distance)
                .FirstOrDefault(p => result.All(chosen =>
                    CalculateDistanceKm(chosen.Lat, chosen.Lon, p.Lat, p.Lon) > 0.15
                ));

            if (best is not null)
                result.Add((best.Lat, best.Lon));
        }

        // Step 4 — close the loop
        result.Add((start.Latitude, start.Longitude));

        return result;
    }

    // handles the 350° vs 10° wrap-around problem
    private static double AngleDifference(double a, double b)
    {
        var diff = Math.Abs(a - b) % 360;
        return diff > 180 ? 360 - diff : diff;
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
