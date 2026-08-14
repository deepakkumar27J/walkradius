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

    public async Task<Route> GenerateAsync(Walk walk)
    {
        var radiusKm = walk.Constraint.ToEstimatedDistanceKm();
        var effectiveRadiusKm = radiusKm / 2;

        var (coordinates, totalDistanceMetres) = await _walkingDataService.GetCircularRouteAsync(
            walk.StartPoint, effectiveRadiusKm);

        var totalDistanceKm = totalDistanceMetres / 1000;
        return new Route(walk.Id, coordinates.ToList(), totalDistanceKm);
    }
}
