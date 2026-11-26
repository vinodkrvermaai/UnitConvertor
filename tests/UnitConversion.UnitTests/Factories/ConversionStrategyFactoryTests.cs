using UnitConversion.Application.Factories;
using UnitConversion.Application.Interfaces;
using UnitConversion.Application.Strategies;
using UnitConversion.Domain.Enums;

namespace UnitConversion.UnitTests.Factories;

[TestFixture]
public sealed class ConversionStrategyFactoryTests
{
    [Test]
    public void Get_ReturnsStrategyForEachCategory()
    {
        IConversionStrategy[] strategies =
        [
            new LengthConversionStrategy(),
            new WeightConversionStrategy(),
            new TemperatureConversionStrategy()
        ];
        var factory = new ConversionStrategyFactory(strategies);

        Assert.That(factory.Get(MeasurementCategory.Length), Is.InstanceOf<LengthConversionStrategy>());
        Assert.That(factory.Get(MeasurementCategory.Weight), Is.InstanceOf<WeightConversionStrategy>());
        Assert.That(factory.Get(MeasurementCategory.Temperature), Is.InstanceOf<TemperatureConversionStrategy>());
    }
}