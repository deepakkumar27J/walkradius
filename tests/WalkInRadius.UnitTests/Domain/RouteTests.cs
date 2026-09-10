using FluentAssertions;
using WalkInRadius.Domain.ValueObjects;
using WalkInRadius.Domain.Entities;

namespace WalkInRadius.UnitTests.Domain;

public class RouteTests
{

    private static List<Coordinate> ValidWaypoints() =>
    [
        new(54.5973, -5.9301),
        new(54.6012, -5.9280),
        new(54.5990, -5.9350),
        new(54.5973, -5.9301)
    ];

    [Fact]
    public void Constructor_WithValidVWaypoints_CreatesRoute()
    {
        // Arrange
        var walkId = Guid.NewGuid();
        var waypoints = ValidWaypoints();

        // Act
        var route = new Route(walkId, waypoints, 2.5);

        // Assert
        route.Should().NotBeNull();
        route.WalkId.Should().Be(walkId);
        route.TotalDistanceKm.Should().Be(2.5);
        route.Waypoints.Should().HaveCount(4);
    }
}
