using FluentValidation;
namespace WalkInRadius.Application.UseCases.GenerateWalk;

public class GenerateWalkValidator: AbstractValidator<GenerateWalkCommand>
{
    private static readonly string[] ValidConstraintTypes = ["Duration", "Distance"];

    public GenerateWalkValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180.");

        RuleFor(x => x.ConstraintType)
            .Must(t=> ValidConstraintTypes.Contains(t))
            .WithMessage("ConstraintType must be 'Duration' or 'Distance'.");

        RuleFor(x => x.Value)
            .GreaterThan(0)
            .WithMessage("Value must be greater than 0.");
    }
}
