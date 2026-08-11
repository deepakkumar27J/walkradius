using System;
using System.Collections.Generic;
using System.Text;

namespace WalkInRadius.Application.DTOs;

public record RouteDTO(
    Guid Id,
    double TotalDistanceKm,
    double EstimatedDurationMins,
    IReadOnlyList<CoordinateDTO> Waypoints
);
