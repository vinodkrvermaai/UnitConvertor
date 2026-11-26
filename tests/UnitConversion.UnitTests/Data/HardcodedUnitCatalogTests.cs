using UnitConversion.Domain.Enums;
using UnitConversion.Infrastructure.Data;

namespace UnitConversion.UnitTests.Data;

[TestFixture]
public sealed class HardcodedUnitCatalogTests
{
    private HardcodedUnitCatalog _catalog = null!;

    [SetUp]
    public void SetUp() => _catalog = new HardcodedUnitCatalog();

    [Test]
    public void TryGet_IsCaseInsensitive()
    {
        Assert.That(_catalog.TryGet("METER", out var unit), Is.True);
        Assert.That(unit.Symbol, Is.EqualTo("meter"));
    }

    [Test]
    public void GetAll_ContainsAllThreeCategories()
    {
        var categories = _catalog.GetAll().Select(u => u.Category).Distinct().ToList();

        Assert.That(categories, Is.EquivalentTo(new[]
        {
            MeasurementCategory.Length,
            MeasurementCategory.Temperature,
            MeasurementCategory.Weight
        }));
    }

    [Test]
    public void GetByCategory_Length_IsSortedByFactor()
    {
        var units = _catalog.GetByCategory(MeasurementCategory.Length);

        Assert.That(units.First().Symbol, Is.EqualTo("micrometer"));
        Assert.That(units.Last().Symbol, Is.EqualTo("nauticalmile"));
    }
}