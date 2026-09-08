using Microsoft.Extensions.Caching.Memory;
using Moq;
using FluentAssertions;
using WalkInRadius.Domain.Entities;
using WalkInRadius.Domain.Enums;
using WalkInRadius.Domain.Interfaces;
using WalkInRadius.Domain.ValueObjects;
using WalkInRadius.Infrastructure.RouteGeneration;

namespace WalkInRadius.UnitTests.Infrastructure;

public class RouteGeneratorTests
{
    private readonly Mock<IWalkingDataService> _mockWalkingDataService;
    private readonly IMemoryCache _cache;
    private readonly RouteGenerator _routeGenerator;

    public RouteGeneratorTests()
    {
        _mockWalkingDataService = new Mock<IWalkingDataService>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _routeGenerator = new RouteGenerator(_mockWalkingDataService.Object, _cache);
    }

    private static Walk BuildWalk(double durationMins = 30) =>
        new(
            new Coordinate(54.5973, -5.9301),
            new WalkConstraint(ConstraintType.Duration, durationMins)
        );

    private static (IEnumerable<Coordinate>, double) FakeRouteData() =>
    (
        new List<Coordinate>
        {
            new(54.5973, -5.9301),
            new(54.6012, -5.9280),
            new(54.5990, -5.9350),
            new(54.5973, -5.9301)
        },
        2500.0 // metres
    );

    [Fact]
    public async Task GenerateAsync_FirstCall_CallsWalkingDataService()
    {
        _mockWalkingDataService
            .Setup(s => s.GetCircularRouteAsync(It.IsAny<Coordinate>(), It.IsAny<double>()))
            .ReturnsAsync(FakeRouteData());

        var walk = BuildWalk();
        await _routeGenerator.GenerateAsync(walk);

        _mockWalkingDataService.Verify(
            s => s.GetCircularRouteAsync(It.IsAny<Coordinate>(), It.IsAny<double>()),
            Times.Once);
    }
}