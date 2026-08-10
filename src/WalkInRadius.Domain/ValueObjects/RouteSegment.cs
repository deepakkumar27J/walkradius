using System;
using System.Collections.Generic;
using System.Text;

namespace WalkInRadius.Domain.ValueObjects;

public record RouteSegment(Coordinate From, Coordinate To, double DistanceKm);

