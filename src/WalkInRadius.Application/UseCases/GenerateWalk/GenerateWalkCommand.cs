using System;
using System.Collections.Generic;
using System.Text;

namespace WalkInRadius.Application.UseCases.GenerateWalk;

public record GenerateWalkCommand(
    double Latitude,
    double Longitude,
    string ConstraintType,
    double Value
    );
