namespace UnitConversion.Application.DTOs;

public sealed class ConversionRequest
{
    public decimal Value { get; set; }

    public string From { get; set; } = string.Empty;

    public string To { get; set; } = string.Empty;
}