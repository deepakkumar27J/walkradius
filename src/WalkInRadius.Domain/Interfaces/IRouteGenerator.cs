using System;
using System.Collections.Generic;
using System.Text;

using WalkInRadius.Domain.Entities;
using WalkInRadius.Domain.ValueObjects;
namespace WalkInRadius.Domain.Interfaces;

public interface IRouteGenerator
{
    Task<Route> GenerateAsync(Walk walk);
}
