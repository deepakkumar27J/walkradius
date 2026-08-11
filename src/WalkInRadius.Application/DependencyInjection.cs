using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WalkInRadius.Application.UseCases.GenerateWalk;
using WalkInRadius.Application.UseCases.GetPlacesInRadius;
namespace WalkInRadius.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services )
    {
        services.AddScoped<GenerateWalkHandler>();
        services.AddScoped<GenerateWalkValidator>();

        return services;
    }
}
