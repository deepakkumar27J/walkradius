using System;
using System.Collections.Generic;
using System.Text;
using WalkInRadius.Domain.Enums;
namespace WalkInRadius.Domain.ValueObjects;

public record WalkConstraint
{
    public ConstraintType Type { get;}
    public double Value { get;}

    private const double MaxDurationMins = 480; //8 hours max needs to be customized
    private const double MaxDistanceKm = 50; // max needs customization

    public WalkConstraint (ConstraintType type,  double value)
    {
        if (value <= 0)
            throw new ArgumentException("Walk constraint value must be greater than 0. ");
        if (type == ConstraintType.Duration && value > MaxDurationMins)
            throw new ArgumentException($"Duration cannot exceed {MaxDurationMins} minutes. ");
        if (type == ConstraintType.Distance && value > MaxDistanceKm)
            throw new ArgumentException($"Distance cannot exceed {MaxDistanceKm} km.");
        
        Type = type;
        Value = value;
    }

    // Convenience: always get the distance in km regardless of constraint type
    // Average walking speed is 5km/h
    public double ToEstimatedDistanceKm() =>
        Type == ConstraintType.Distance ? Value : (Value / 60) * 5.0; // convert mins to hrs and multiply by speed
}

