using System;
using System.Collections.Generic;
using System.Text;

using WalkInRadius.Domain.ValueObjects;

namespace WalkInRadius.Domain.Interfaces;

public interface IWalkingDataService
{
    Task<IEnumerable<Coordinate>> getCircularRouteAsync(Coordinate start, double radiusKm);
}
