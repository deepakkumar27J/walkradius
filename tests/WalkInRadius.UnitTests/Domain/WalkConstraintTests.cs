using FluentAssertions;
using WalkInRadius.Domain.Enums;
using WalkInRadius.Domain.ValueObjects;

namespace WalkInRadius.UnitTests.Domain;

public class WalkConstraintTests
{
    // ─── Valid creation ───────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidDuration_CreatesConstraint()
    {
        var constraint = new WalkConstraint(ConstraintType.Duration, 30);

        constraint.Type.Should().Be(ConstraintType.Duration);
        constraint.Value.Should().Be(30);
    }

    [Fact]
    public void Constructor_WithValidDistance_CreatesConstraint()
    {
        var constraint = new WalkConstraint(ConstraintType.Distance, 2.5);

        constraint.Type.Should().Be(ConstraintType.Distance);
        constraint.Value.Should().Be(2.5);
    }

    // ─── Invalid values ───────────────────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithZeroOrNegativeValue_ThrowsArgumentException(double invalidValue)
    {
        var act = () => new WalkConstraint(ConstraintType.Duration, invalidValue);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*greater than 0*");
    }

    [Fact]
    public void Constructor_WithDurationExceedingMax_ThrowsArgumentException()
    {
        var act = () => new WalkConstraint(ConstraintType.Duration, 481);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*480*");
    }

    [Fact]
    public void Constructor_WithDistanceExceedingMax_ThrowsArgumentException()
    {
        var act = () => new WalkConstraint(ConstraintType.Distance, 51);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*50*");
    }

    // ─── ToEstimatedDistanceKm ────────────────────────────────────────────────

    [Fact]
    public void ToEstimatedDistanceKm_WhenTypeIsDistance_ReturnsValueDirectly()
    {
        var constraint = new WalkConstraint(ConstraintType.Distance, 3.0);

        constraint.ToEstimatedDistanceKm().Should().Be(3.0);
    }

    [Theory]
    [InlineData(60, 5.0)]   // 60 mins at 5km/h = 5km
    [InlineData(30, 2.5)]   // 30 mins at 5km/h = 2.5km
    [InlineData(120, 10.0)] // 120 mins at 5km/h = 10km
    [InlineData(15, 1.25)]  // 15 mins at 5km/h = 1.25km
    public void ToEstimatedDistanceKm_WhenTypeIsDuration_ConvertsCorrectly(
        double minutes, double expectedKm)
    {
        var constraint = new WalkConstraint(ConstraintType.Duration, minutes);

        constraint.ToEstimatedDistanceKm().Should().BeApproximately(expectedKm, precision: 0.001);
    }
}