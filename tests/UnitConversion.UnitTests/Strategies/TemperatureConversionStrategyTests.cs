using UnitConversion.Application.Strategies;
using UnitConversion.Domain.Entities;
using UnitConversion.Domain.Enums;

namespace UnitConversion.UnitTests.Strategies;

[TestFixture]
public sealed class TemperatureConversionStrategyTests
{
    private TemperatureConversionStrategy _strategy = null!;

    [SetUp]
    public void SetUp() => _strategy = new TemperatureConversionStrategy();

    [Test]
    public void Convert_ZeroCelsiusToFahrenheit_Returns32()
    {
        var celsius = Affine("celsius", 1m, 0m);
        var fahrenheit = Affine("fahrenheit", 5m / 9m, -160m / 9m);

        var result = _strategy.Convert(0m, celsius, fahrenheit);

        Assert.That(result, Is.EqualTo(32m).Within(0.0001m));
    }

    [Test]
    public void Convert_100CelsiusToFahrenheit_Returns212()
    {
        var celsius = Affine("celsius", 1m, 0m);
        var fahrenheit = Affine("fahrenheit", 5m / 9m, -160m / 9m);

        var result = _strategy.Convert(100m, celsius, fahrenheit);

        Assert.That(result, Is.EqualTo(212m).Within(0.0001m));
    }

    private static UnitDefinition Affine(string symbol, decimal mult, decimal add) => new()
    {
        Symbol = symbol,
        DisplayName = symbol,
        Category = MeasurementCategory.Temperature,
        Kind = ConversionKind.Affine,
        ToCelsiusMultiply = mult,
        ToCelsiusAdd = add
    };
}