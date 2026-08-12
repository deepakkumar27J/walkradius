using System;
using System.Collections.Generic;
using System.Text;

namespace WalkInRadius.Infrastructure.ExternalApis.FourSquarePlaces.Models;

public class OrsIsochroneRequest
{
    public List<List<double>> Locations { get; set; } = [];
    public string Range_type { get; set; } = "time";
    public List<double> Range { get; set; } = [];
}
