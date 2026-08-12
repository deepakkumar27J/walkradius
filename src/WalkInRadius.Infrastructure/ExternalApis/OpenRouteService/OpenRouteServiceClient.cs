using System.Text.Json;
using System.Collections.Generic;
using System.Text;
using WalkInRadius.Infrastructure.ExternalApis.OpenRouteService.Models;
using WalkInRadius.Infrastructure.ExternalApis.FourSquarePlaces.Models;

namespace WalkInRadius.Infrastructure.ExternalApis.FourSquarePlaces;

public class OpenRouteServiceClient
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public OpenRouteServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    // Isochrone
    public async Task<OrsIsochroneResponse?> GetIsochroneAsync(
        double latitude,
        double longitude,
        double rangeSeconds)
    {
        var request = new OrsIsochroneRequest
        {
            Locations = [[longitude, latitude]],
            Range_type = "time",
            Range = [rangeSeconds]
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("v2/isochrones/foot-walking", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<OrsIsochroneResponse>(responseJson, JsonOpts);
    }

    //Directions
    public async Task<OrsIsochroneDirectionsResponse?> GetDirectionsAsync(
        IEnumerable<(double Latitude, double Longitude)> waypoints)
    {
        var coordinates = waypoints
            .Select(w => new List<double> { w.Longitude, w.Latitude })
            .ToList();

        var body = new { coordinates };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("v2/directions/foot-walking/geojson", content);
        response.EnsureSuccessStatusCode() ;

        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<OrsIsochroneDirectionsResponse>(responseJson, JsonOpts);
    }
}
