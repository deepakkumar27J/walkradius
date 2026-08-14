using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WalkInRadius.Infrastructure.ExternalApis.FourSquarePlaces.Models;

public class OrsIsochroneRequest
{
    [JsonPropertyName("locations")]
    public List<List<double>> Locations { get; set; } = [];

    [JsonPropertyName("range_type")]
    public string RangeType { get; set; } = "time";

    [JsonPropertyName("range")]
    public List<double> Range { get; set; } = [];
}
