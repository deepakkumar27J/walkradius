using System;
using System.Collections.Generic;
using System.Text;

namespace WalkInRadius.Domain.Exceptions;

public class InvalidWalkConstraintException : Exception
{
    public InvalidWalkConstraintException (string message) : base (message) { }
}
