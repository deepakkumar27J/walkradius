using System;
using System.Collections.Generic;
using System.Text;
using WalkInRadius.Domain.ValueObjects;
namespace WalkInRadius.Domain.Entities;
public class Walk
{
    public Guid Id { get;}
    public Coordinate StartPoint {  get;}
    public WalkConstraint Constraint { get;}
    public DateTime CreatedAt { get;}

    public Walk(Coordinate startPoint, WalkConstraint constraint)
    {
        Id = Guid.NewGuid();
        StartPoint = startPoint;
        Constraint = constraint;
        CreatedAt = DateTime.UtcNow;
    }
}

