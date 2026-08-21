using FluentAssertions;
using Moq;
using WalkInRadius.Application.UseCases.GenerateWalk;
using WalkInRadius.Application.UseCases.GetPlacesInRadius;
using WalkInRadius.Domain.Entities;
using WalkInRadius.Domain.Enums;
using WalkInRadius.Domain.Interfaces;
using WalkInRadius.Domain.ValueObjects;

namespace WalkInRadius.UnitTests.Application;

public class GenerateWalkHandlerTests
{
    // ─── Setup ────────────────────────────────────────────────────────────────

    private readonly Mock<IRouteGenerator> _mockRouteGenerator;
    private readonly GenerateWalkHandler _handler;

    public GenerateWalkHandlerTests()
    {
        _mockRouteGenerator = new Mock<IRouteGenerator>();
        _handler = new GenerateWalkHandler(_mockRouteGenerator.Object, new GenerateWalkValidator());
    }

    // ─── Helper ───────────────────────────────────────────────────────────────

    private static Route BuildFakeRoute(Guid walkId)
    {
        var waypoints = new List<Coordinate>
        {
            new(54.5973, -5.9301),
            new(54.6012, -5.9280),
            new(54.5990, -5.9350),
            new(54.5973, -5.9301)
        };
        return new Route(walkId, waypoints, 2.5);
    }

    // ─── Success cases ────────────────────────────────────────────────────────

    [Fact]
    public async Task HandleAsync_WithValidDurationCommand_ReturnsRouteDto()
    {
        // Arrange
        var command = new GenerateWalkCommand(54.5973, -5.9301, "Duration", 30);

        _mockRouteGenerator
            .Setup(r => r.GenerateAsync(It.IsAny<Walk>()))
            .ReturnsAsync((Walk walk) => BuildFakeRoute(walk.Id));

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.TotalDistanceKm.Should().Be(2.5);
        result.Waypoints.Should().HaveCount(4);
        result.EstimatedDurationMins.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task HandleAsync_WithValidDistanceCommand_ReturnsRouteDto()
    {
        var command = new GenerateWalkCommand(54.5973, -5.9301, "Distance", 3.0);

        _mockRouteGenerator
            .Setup(r => r.GenerateAsync(It.IsAny<Walk>()))
            .ReturnsAsync((Walk walk) => BuildFakeRoute(walk.Id));

        var result = await _handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.Waypoints.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_CallsRouteGeneratorOnce()
    {
        var command = new GenerateWalkCommand(54.5973, -5.9301, "Duration", 30);

        _mockRouteGenerator
            .Setup(r => r.GenerateAsync(It.IsAny<Walk>()))
            .ReturnsAsync((Walk walk) => BuildFakeRoute(walk.Id));

        await _handler.HandleAsync(command);

        // Verify IRouteGenerator was called exactly once
        _mockRouteGenerator.Verify(r => r.GenerateAsync(It.IsAny<Walk>()), Times.Once);
    }

    // ─── Validation failure cases ─────────────────────────────────────────────

    [Theory]
    [InlineData(200, -5.9301, "Duration", 30)]   // invalid latitude
    [InlineData(54.5973, 200, "Duration", 30)]    // invalid longitude
    [InlineData(54.5973, -5.9301, "Duration", 0)] // zero value
    [InlineData(54.5973, -5.9301, "Sprinting", 30)] // invalid constraint type
    public async Task HandleAsync_WithInvalidCommand_ThrowsValidationException(
        double lat, double lon, string type, double value)
    {
        var command = new GenerateWalkCommand(lat, lon, type, value);

        var act = () => _handler.HandleAsync(command);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async Task HandleAsync_WithInvalidCommand_NeverCallsRouteGenerator()
    {
        var command = new GenerateWalkCommand(200, -5.9301, "Duration", 30);

        try { await _handler.HandleAsync(command); } catch { }

        // Generator should never be called if validation fails
        _mockRouteGenerator.Verify(
            r => r.GenerateAsync(It.IsAny<Walk>()),
            Times.Never);
    }
}