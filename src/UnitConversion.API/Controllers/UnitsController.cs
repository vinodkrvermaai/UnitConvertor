using Microsoft.AspNetCore.Mvc;
using UnitConversion.Application.DTOs;
using UnitConversion.Application.Services;
using UnitConversion.Domain.Enums;

namespace UnitConversion.API.Controllers;

[ApiController]
[Route("api/v1")]
public sealed class UnitsController : ControllerBase
{
    private readonly UnitCatalogService _catalogService;

    public UnitsController(UnitCatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet("units")]
    [ProducesResponseType(typeof(IEnumerable<UnitResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<IEnumerable<UnitResponse>> GetUnits([FromQuery] string? category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return Ok(_catalogService.GetUnits());
        }

        if (!Enum.TryParse<MeasurementCategory>(category, ignoreCase: true, out var parsed))
        {
            return Problem(
                detail: $"Unknown category '{category}'. Valid values: Length, Temperature, Weight.",
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid category");
        }

        return Ok(_catalogService.GetUnits(parsed));
    }

    [HttpGet("categories")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<string>> GetCategories() =>
        Ok(_catalogService.GetCategoryNames());
}