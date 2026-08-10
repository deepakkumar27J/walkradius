using System;
using System.Collections.Generic;
using System.Text;
using WalkInRadius.Domain.ValueObjects;
namespace WalkInRadius.Domain.Entities;
public class Route
{
    public Guid Id { get;}
    public Guid WalkId { get;}
    public IReadOnlyList<Coordinate> Waypoints { get; }
    public double TotalDistanceKm { get; }
    public double EstimatedDurationMins { get; }

    private const double AverageWalkingSpeedKmh = 5.0;

    public Route (Guid walkId, IReadOnlyList<Coordinate> waypoints, double totalDistanceKm)
    {
        if (waypoints.Count < 2)
            throw new ArgumentException("A route must have been at least 2 waypoints. ");

        Id = Guid.NewGuid();
        WalkId = walkId;
        Waypoints = waypoints;
        TotalDistanceKm = totalDistanceKm;
        EstimatedDurationMins = (totalDistanceKm / AverageWalkingSpeedKmh) * 60;
    }
}

