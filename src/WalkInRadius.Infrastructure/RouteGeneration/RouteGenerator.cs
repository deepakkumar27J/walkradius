using System;
using System.Collections.Generic;
using System.Text;
using WalkInRadius.Domain.Entities;
using WalkInRadius.Domain.Enums;
using WalkInRadius.Domain.Interfaces;

namespace WalkInRadius.Infrastructure.RouteGeneration;

public class RouteGenerator : IRouteGenerator
{
    private readonly IWalkingDataService _walkingDataService;

    public RouteGenerator(IWalkingDataService walkingDataService)
    {
        _walkingDataService = walkingDataService;
    }

    public async Task<Route> GenerateAsync (Walk walk)
    {
        var radiusKm = walk.Constraint.ToEstimatedDistanceKm();

        // For a circular walk, halve the radius so the total route
        // fits within the constraint
        var effectiveRadiusKm = radiusKm / 2;

        var coordinates = await _walkingDataService.getCircularRouteAsync(
            walk.StartPoint,
            effectiveRadiusKm);

        var waypoints = coordinates.ToList();

        //Calculate total distance from the directions response
        // for now estimate from radius
        var totalDistanceKm = effectiveRadiusKm * 2;

        return new Route(walk.Id, waypoints, totalDistanceKm);
    }
}
