using System;
using System.Collections.Generic;
using System.Text;

namespace WalkInRadius.Infrastructure.ExternalApis.OpenRouteService.Models;

public class OrsIsochroneDirectionsResponse
{
    public List<OrsRoute> Routes { get; set; } = [];
}

public class OrsRoute
{
    public OrsSummary Summary { get; set; } = new();
    public OrsGeometry Geometry { get; set; } = new();
}

public class OrsSummary
{
    public double Distance { get; set; }
    public double Duration { get; set; }
}

public class OrsGeometry
{
    public List<List<double>> Coordinates { get; set; } = [];
}