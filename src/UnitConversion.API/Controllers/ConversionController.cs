using Microsoft.AspNetCore.Mvc;
using UnitConversion.Application.DTOs;
using UnitConversion.Application.Services;

namespace UnitConversion.API.Controllers;

[ApiController]
[Route("api/v1")]
public sealed class ConversionController : ControllerBase
{
    private readonly UnitConversionService _conversionService;

    public ConversionController(UnitConversionService conversionService)
    {
        _conversionService = conversionService;
    }

    // Query binding + FluentValidation run before this; invalid requests return 400 without calling the service.
    [HttpGet("conversion")]
    [ProducesResponseType(typeof(ConversionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<ConversionResponse> Convert([FromQuery] ConversionRequest request) =>
        Ok(_conversionService.Convert(request));
}