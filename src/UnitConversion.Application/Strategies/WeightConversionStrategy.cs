using UnitConversion.Application.Interfaces;
using UnitConversion.Domain.Entities;
using UnitConversion.Domain.Enums;

namespace UnitConversion.Application.Strategies;

public sealed class WeightConversionStrategy : IConversionStrategy
{
    public MeasurementCategory Category => MeasurementCategory.Weight;

    public decimal Convert(decimal value, UnitDefinition from, UnitDefinition to)
    {
        if (from.Category != Category || to.Category != Category
            || from.Kind != ConversionKind.Linear || to.Kind != ConversionKind.Linear
            || from.FactorToBase is null || to.FactorToBase is null)
        {
            throw new InvalidOperationException("Weight conversion requires linear weight units with factors.");
        }

        var inBase = value * from.FactorToBase.Value;
        return inBase / to.FactorToBase.Value;
    }
}