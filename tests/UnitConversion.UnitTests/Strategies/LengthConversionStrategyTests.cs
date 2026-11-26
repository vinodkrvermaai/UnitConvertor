using UnitConversion.Application.Strategies;
using UnitConversion.Domain.Entities;
using UnitConversion.Domain.Enums;

namespace UnitConversion.UnitTests.Strategies;

[TestFixture]
public sealed class LengthConversionStrategyTests
{
    private LengthConversionStrategy _strategy = null!;

    [SetUp]
    public void SetUp() => _strategy = new LengthConversionStrategy();

    [Test]
    public void Convert_OneMeterToFoot_ReturnsExpected()
    {
        var meter = Linear("meter", 1m);
        var foot = Linear("foot", 0.3048m);

        var result = _strategy.Convert(1m, meter, foot);

        Assert.That(result, Is.EqualTo(3.280839895m).Within(0.0001m));
    }

    private static UnitDefinition Linear(string symbol, decimal factor) => new()
    {
        Symbol = symbol,
        DisplayName = symbol,
        Category = MeasurementCategory.Length,
        Kind = ConversionKind.Linear,
        FactorToBase = factor
    };
}