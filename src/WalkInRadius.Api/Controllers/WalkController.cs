
using Microsoft.AspNetCore.Mvc;
using WalkInRadius.Application.DTOs;
using WalkInRadius.Application.UseCases.GenerateWalk;
using WalkInRadius.Application.UseCases.GetPlacesInRadius;
namespace WalkInRadius.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalkController: ControllerBase
{
    private readonly GenerateWalkHandler _handler;
    private readonly ILogger<WalkController> _logger;

    public WalkController(GenerateWalkHandler handler, ILogger<WalkController> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    //Generate a circular walking route based on a starting location and time or distance constraint

    [HttpPost]
    [ProducesResponseType(typeof(RouteDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> GenerateWalk([FromBody] GenerateWalkCommand command)
    {
        _logger.LogInformation(
            "Generating walk from ({Lat}, {Lon}) with {Type} constraint of {Value}",
            command.Latitude, command.Longitude, command.ConstraintType, command.Value);
        var result = await _handler.HandleAsync(command);

        return Ok(result);
    }
}
