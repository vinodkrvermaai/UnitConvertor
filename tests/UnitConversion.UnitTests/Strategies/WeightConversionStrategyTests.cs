using UnitConversion.Application.Strategies;
using UnitConversion.Domain.Entities;
using UnitConversion.Domain.Enums;

namespace UnitConversion.UnitTests.Strategies;

[TestFixture]
public sealed class WeightConversionStrategyTests
{
    [Test]
    public void Convert_OneKilogramToPound_ReturnsExpected()
    {
        var strategy = new WeightConversionStrategy();
        var kg = Linear("kilogram", 1m);
        var lb = Linear("pound", 0.45359237m);

        var result = strategy.Convert(1m, kg, lb);

        Assert.That(result, Is.EqualTo(2.20462m).Within(0.001m));
    }

    private static UnitDefinition Linear(string symbol, decimal factor) => new()
    {
        Symbol = symbol,
        DisplayName = symbol,
        Category = MeasurementCategory.Weight,
        Kind = ConversionKind.Linear,
        FactorToBase = factor
    };
}