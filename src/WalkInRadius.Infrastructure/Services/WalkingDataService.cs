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

    public async Task<IEnumerable<Coordinate>> getCircularRouteAsync(
        Coordinate start,
        double radiusKm)
    {
        var rangeSeconds = (radiusKm / 5.0) * 3600;

        // get reachable area
        var isochrone = await _orsClient.GetIsochroneAsync(
            start.Latitude,
            start.Longitude,
            rangeSeconds);

        if (isochrone?.Features is null || isochrone.Features.Count == 0)
        {
            throw new InvalidOperationException("Could not get walkable area from routing service.");
        }

        // pick waypoints from the polygon boundry
        var polygon = isochrone.Features[0].Geometry.Coordinates[0];
        var waypoints = PickWaypoints(start, polygon);

        //get direction between the waypoints
        var directions = await _orsClient.GetDirectionsAsync(waypoints);

        if (directions?.Routes is null || directions.Routes.Count == 0)
            throw new InvalidOperationException("Could not generate walking route.");

        //map ORS coordinates back to domain Coordinates
        return directions.Routes[0].Geometry.Coordinates
            .Select(c => new Coordinate(c[1], c[0]))
            .ToList();
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

}
