namespace WalkInRadius.Infrastructure.ExternalApis.OpenRouteService.Models;

public class OrsIsochroneDirectionsResponse
{
    public List<OrsDirectionsFeature> Features { get; set; } = [];
}

public class OrsDirectionsFeature
{
    public OrsDirectionsGeometry Geometry { get; set; } = new();
    public OrsDirectionsProperties Properties { get; set; } = new();
}

public class OrsDirectionsGeometry
{
    public List<List<double>> Coordinates { get; set; } = [];
}

public class OrsDirectionsProperties
{
    public OrsSummary Summary { get; set; } = new();
}

public class OrsSummary
{
    public double Distance { get; set; }
    public double Duration { get; set; }
}