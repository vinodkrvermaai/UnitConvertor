namespace UnitConversion.Application.DTOs;

public sealed class ConversionResponse
{
    public decimal Value { get; init; }

    public string From { get; init; } = string.Empty;

    public string To { get; init; } = string.Empty;

    public decimal Result { get; init; }

    public string Category { get; init; } = string.Empty;
}