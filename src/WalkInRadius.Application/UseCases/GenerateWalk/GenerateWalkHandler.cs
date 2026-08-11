using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using WalkInRadius.Application.DTOs;
using WalkInRadius.Application.UseCases.GenerateWalk;
using WalkInRadius.Domain.Entities;
using WalkInRadius.Domain.Enums;
using WalkInRadius.Domain.Interfaces;
using WalkInRadius.Domain.ValueObjects;
namespace WalkInRadius.Application.UseCases.GetPlacesInRadius;

public class GenerateWalkHandler
{
    private readonly IRouteGenerator _routeGenerator;
    private readonly GenerateWalkValidator _validator;

    public GenerateWalkHandler(IRouteGenerator routeGenerator, GenerateWalkValidator validator)
    {
        _routeGenerator = routeGenerator;
        _validator = validator;
    }

    public async Task<RouteDTO> HandleAsync(GenerateWalkCommand command)
    {
        var validationResult = await _validator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(errors);
        }

        var coordinate = new Coordinate(command.Latitude, command.Longitude);
        var constraintType = Enum.Parse<ConstraintType>(command.ConstraintType);
        var constraint = new WalkConstraint(constraintType, command.Value);

        var walk = new Walk(coordinate, constraint);

        var route = await _routeGenerator.GenerateAsync(walk);

        return MapToDto(route);
    }

    private static RouteDTO MapToDto(Route route) =>
        new(
            route.Id,
            Math.Round(route.TotalDistanceKm, 2),
            Math.Round(route.EstimatedDurationMins, 0),
            route.Waypoints
                .Select(w => new CoordinateDTO(w.Latitude, w.Longitude))
                .ToList()
            );
}
