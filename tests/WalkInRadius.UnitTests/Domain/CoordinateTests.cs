using FluentAssertions;
using WalkInRadius.Domain.ValueObjects;

namespace WalkInRadius.UnitTests.Domain;

public class CoordinateTests
{
    // Valid creation

    [Fact]
    public void Constructor_WithValidValues_CreatesCoordinates()
    {
        var coordinate = new Coordinate(54.5973, -5.9301);

        coordinate.Latitude.Should().Be(54.5973);
        coordinate.Longitude.Should().Be(-5.9301);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, -180)]
    [InlineData(-90, -180)]
    [InlineData(51.5, -0.1)]   // London
    [InlineData(54.5973, -5.9301)] // Belfast
    public void Constructor_WithBoundaryValues_CreatesCoordinate(double lat, double lon)
    {
        var act = () => new Coordinate(lat, lon);
        act.Should().NotThrow();
    }
}
