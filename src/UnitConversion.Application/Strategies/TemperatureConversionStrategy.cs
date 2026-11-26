using UnitConversion.Application.Interfaces;
using UnitConversion.Domain.Entities;
using UnitConversion.Domain.Enums;

namespace UnitConversion.Application.Strategies;

public sealed class TemperatureConversionStrategy : IConversionStrategy
{
    public MeasurementCategory Category => MeasurementCategory.Temperature;

    public decimal Convert(decimal value, UnitDefinition from, UnitDefinition to)
    {
        if (from.Category != Category || to.Category != Category
            || from.Kind != ConversionKind.Affine || to.Kind != ConversionKind.Affine
            || from.ToCelsiusMultiply is null || from.ToCelsiusAdd is null
            || to.ToCelsiusMultiply is null || to.ToCelsiusAdd is null)
        {
            throw new InvalidOperationException("Temperature conversion requires affine units with Celsius coefficients.");
        }

        // All temperature units route through Celsius.
        var celsius = value * from.ToCelsiusMultiply.Value + from.ToCelsiusAdd.Value;
        return (celsius - to.ToCelsiusAdd.Value) / to.ToCelsiusMultiply.Value;
    }
}