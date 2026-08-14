using System;
using System.Collections.Generic;
using System.Text;

using WalkInRadius.Domain.ValueObjects;

namespace WalkInRadius.Domain.Interfaces;

public interface IWalkingDataService
{
    Task<(IEnumerable<Coordinate> Waypoints, double TotalDistanceMetres)> GetCircularRouteAsync(
        Coordinate start, double radiusKm);
}
