using UnitConversion.Domain.Entities;
using UnitConversion.Domain.Enums;

namespace UnitConversion.Application.Interfaces;

public interface IConversionStrategy
{
    MeasurementCategory Category { get; }

    decimal Convert(decimal value, UnitDefinition from, UnitDefinition to);
}