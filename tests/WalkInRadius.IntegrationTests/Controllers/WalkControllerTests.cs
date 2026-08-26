using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Moq;
using WalkInRadius.Application.DTOs;
using WalkInRadius.Domain.Entities;
using WalkInRadius.Domain.ValueObjects;
using WalkInRadius.IntegrationTests.Factories;
using System.Net.Http.Json;
using WalkInRadius.Domain.Enums;

namespace WalkInRadius.IntegrationTests.Controllers;

public class WalkControllerTests
{
    private readonly HttpClient _client;
    private readonly WalkApiFactory _factory;

    public WalkControllerTests(WalkApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static StringContent JsonBody(object obj) =>
        new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    private static List<Coordinate> FakeWaypoints() =>
    [
        new(54.5973, -5.9301),
        new(54.6012, -5.9280),
        new(54.5990, -5.9350),
        new(54.5973, -5.9301)
    ];

    private void SetupMockRoute(double DistanceKm = 2.5)
    {
        _factory.RouteGeneratorMock
            .Setup(r => r.GenerateAsync(It.IsAny<Walk>()))
            .ReturnsAsync((Walk walk) => new Route(walk.Id, FakeWaypoints(), DistanceKm));
    }

    //Success cases
    [Fact]
    public async Task POST_Walk_WithValidDurationRequest_Returns200()
    {
        SetupMockRoute();

        var body = JsonBody(new
        {
            latitude = 54.5973,
            longitude = -5.9301,
            constraintType = "Duration",
            value = 30
        });

        var response = await _client.PostAsync("/api/walk", body);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task POST_Walk_WithValidRequest_ReturnsRouteDTOShape()
    {
        SetupMockRoute(2.5);

        var body = JsonBody(new
        {
            latitude = 54.5973,
            longitude = -5.9301,
            constraintType = "Duration",
            value = 30
        });

        var response = await _client.PostAsync("/api/walk", body);
        var result = await response.Content.ReadFromJsonAsync<RouteDTO>();

        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.TotalDistanceKm.Should().BeGreaterThan(0);
        result.EstimatedDurationMins.Should().BeGreaterThan(0);
        result.Waypoints.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task POST_Walk_WithDistanceConstraint_Returns200()
    {
        SetupMockRoute(3.0);
        var body = JsonBody(new
        {
            latitude = 54.5973,
            longitude = -5.9301,
            ConstraintType = "Distance",
            value = 3.0
        });

        var response = await _client.PostAsync("/api/walk", body);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
