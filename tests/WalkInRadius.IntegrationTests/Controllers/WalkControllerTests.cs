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
}
