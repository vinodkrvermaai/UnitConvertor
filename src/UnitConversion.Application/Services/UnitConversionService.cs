using System.Diagnostics;
using Microsoft.Extensions.Logging;
using UnitConversion.Application.DTOs;
using UnitConversion.Application.Factories;
using UnitConversion.Application.Interfaces;

namespace UnitConversion.Application.Services;

public sealed class UnitConversionService
{
    private readonly IUnitCatalog _catalog;
    private readonly ConversionStrategyFactory _strategyFactory;
    private readonly ILogger<UnitConversionService> _logger;

    public UnitConversionService(
        IUnitCatalog catalog,
        ConversionStrategyFactory strategyFactory,
        ILogger<UnitConversionService> logger)
    {
        _catalog = catalog;
        _strategyFactory = strategyFactory;
        _logger = logger;
    }

    // Assumes HTTP validation already ran; logs audit lines only for conversion attempts that reach here.
    public ConversionResponse Convert(ConversionRequest request)
    {
        var fromSymbol = request.From.Trim();
        var toSymbol = request.To.Trim();
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "Starting conversion of {Value} from {From} to {To}",
            request.Value, fromSymbol, toSymbol);

        // Resolve units → strategy by category → convert → round for API response.
        if (!_catalog.TryGet(fromSymbol, out var fromUnit)
            || !_catalog.TryGet(toSymbol, out var toUnit))
        {
            throw new InvalidOperationException(
                "UnitConversionService expects registered units; validate requests before calling Convert.");
        }

        if (fromUnit.Category != toUnit.Category)
        {
            throw new InvalidOperationException(
                "UnitConversionService expects units in the same category; validate requests before calling Convert.");
        }

        var strategy = _strategyFactory.Get(fromUnit.Category);
        var raw = strategy.Convert(request.Value, fromUnit, toUnit);
        var result = Math.Round(raw, 6, MidpointRounding.AwayFromZero);

        stopwatch.Stop();
        _logger.LogInformation(
            "Converted {Value} from {From} to {To}, result {Result} in {ElapsedMs}ms",
            request.Value, fromSymbol, toSymbol, result, stopwatch.Elapsed.TotalMilliseconds);

        return new ConversionResponse
        {
            Value = request.Value,
            From = fromUnit.Symbol,
            To = toUnit.Symbol,
            Result = result,
            Category = fromUnit.Category.ToString()
        };
    }
}