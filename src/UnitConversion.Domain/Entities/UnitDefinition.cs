using UnitConversion.Domain.Enums;

namespace UnitConversion.Domain.Entities;

public sealed class UnitDefinition
{
    public required string Symbol { get; init; }

    public required string DisplayName { get; init; }

    public required MeasurementCategory Category { get; init; }

    public required ConversionKind Kind { get; init; }

    // Linear: value in category base = value * FactorToBase (meter or kilogram).
    public decimal? FactorToBase { get; init; }

    // Affine (temperature): celsius = value * ToCelsiusMultiply + ToCelsiusAdd.
    public decimal? ToCelsiusMultiply { get; init; }

    public decimal? ToCelsiusAdd { get; init; }
}