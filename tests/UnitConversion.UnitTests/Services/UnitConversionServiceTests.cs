using Microsoft.Extensions.Logging.Abstractions;
using UnitConversion.Application.DTOs;
using UnitConversion.Application.Factories;
using UnitConversion.Application.Interfaces;
using UnitConversion.Application.Services;
using UnitConversion.Application.Strategies;
using UnitConversion.Infrastructure.Data;

namespace UnitConversion.UnitTests.Services;

[TestFixture]
public sealed class UnitConversionServiceTests
{
    private UnitConversionService _service = null!;

    [SetUp]
    public void SetUp()
    {
        IUnitCatalog catalog = new HardcodedUnitCatalog();
        var factory = new ConversionStrategyFactory(
        [
            new LengthConversionStrategy(),
            new WeightConversionStrategy(),
            new TemperatureConversionStrategy()
        ]);
        _service = new UnitConversionService(catalog, factory, NullLogger<UnitConversionService>.Instance);
    }

    [Test]
    public void Convert_MeterToFoot_ReturnsRoundedResult()
    {
        var response = _service.Convert(new ConversionRequest { Value = 1, From = "meter", To = "foot" });

        Assert.That(response.Result, Is.EqualTo(3.280840m));
        Assert.That(response.Category, Is.EqualTo("Length"));
    }

    [Test]
    public void Convert_UnregisteredUnit_ThrowsInvalidOperationException()
    {
        var act = () => _service.Convert(new ConversionRequest { Value = 1, From = "xyz", To = "meter" });

        Assert.That(act, Throws.TypeOf<InvalidOperationException>());
    }
}