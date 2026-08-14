namespace WalkInRadius.Infrastructure.ExternalApis.OpenRouteService.Models;

public class OrsIsochroneResponse
{
    public List<OrsIsochroneFeature> Features { get; set; } = [];
}

public class OrsIsochroneFeature
{
    public OrsIsochroneGeometry Geometry { get; set; } = new();
}

public class OrsIsochroneGeometry
{
    public string Type { get; set; } = string.Empty;
    public List<List<List<double>>> Coordinates { get; set; } = [];
}