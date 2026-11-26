using UnitConversion.Application.Interfaces;
using UnitConversion.Domain.Entities;
using UnitConversion.Domain.Enums;

namespace UnitConversion.Infrastructure.Data;

public sealed class HardcodedUnitCatalog : IUnitCatalog
{
    private readonly Dictionary<string, UnitDefinition> _units;

    public HardcodedUnitCatalog()
    {
        _units = BuildAll()
            .ToDictionary(u => u.Symbol, u => u, StringComparer.OrdinalIgnoreCase);
    }

    public bool TryGet(string symbol, out UnitDefinition unit)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            unit = null!;
            return false;
        }

        return _units.TryGetValue(symbol.Trim(), out unit!);
    }

    public IReadOnlyList<UnitDefinition> GetAll() =>
        _units.Values
            .OrderBy(u => u.Category)
            .ThenBy(u => u, Comparer<UnitDefinition>.Create(CompareWithinCategory))
            .ToList();

    public IReadOnlyList<UnitDefinition> GetByCategory(MeasurementCategory category) =>
        _units.Values
            .Where(u => u.Category == category)
            .OrderBy(u => u, Comparer<UnitDefinition>.Create(CompareWithinCategory))
            .ToList();

    public IReadOnlyList<MeasurementCategory> GetCategories() =>
        _units.Values.Select(u => u.Category).Distinct().OrderBy(c => c.ToString()).ToList();

    private static IEnumerable<UnitDefinition> BuildAll()
    {
        foreach (var u in BuildLength())
        {
            yield return u;
        }

        foreach (var u in BuildWeight())
        {
            yield return u;
        }

        foreach (var u in BuildTemperature())
        {
            yield return u;
        }
    }

    private static IEnumerable<UnitDefinition> BuildLength()
    {
        yield return LinearLength("micrometer", "Micrometer", 0.000001m);
        yield return LinearLength("millimeter", "Millimeter", 0.001m);
        yield return LinearLength("centimeter", "Centimeter", 0.01m);
        yield return LinearLength("inch", "Inch", 0.0254m);
        yield return LinearLength("foot", "Foot", 0.3048m);
        yield return LinearLength("yard", "Yard", 0.9144m);
        yield return LinearLength("meter", "Meter", 1m);
        yield return LinearLength("kilometer", "Kilometer", 1000m);
        yield return LinearLength("mile", "Mile", 1609.344m);
        yield return LinearLength("nauticalmile", "Nautical mile", 1852m);
    }

    private static IEnumerable<UnitDefinition> BuildWeight()
    {
        yield return LinearWeight("milligram", "Milligram", 0.000001m);
        yield return LinearWeight("gram", "Gram", 0.001m);
        yield return LinearWeight("ounce", "Ounce", 0.028349523125m);
        yield return LinearWeight("pound", "Pound", 0.45359237m);
        yield return LinearWeight("kilogram", "Kilogram", 1m);
        yield return LinearWeight("metricton", "Metric ton", 1000m);
    }

    private static IEnumerable<UnitDefinition> BuildTemperature()
    {
        const decimal fiveNinths = 5m / 9m;

        yield return Affine("celsius", "Celsius", 1m, 0m);
        yield return Affine("fahrenheit", "Fahrenheit", fiveNinths, -160m / 9m);
        yield return Affine("kelvin", "Kelvin", 1m, -273.15m);
    }

    private static UnitDefinition LinearLength(string symbol, string name, decimal factorToBase) => new()
    {
        Symbol = symbol,
        DisplayName = name,
        Category = MeasurementCategory.Length,
        Kind = ConversionKind.Linear,
        FactorToBase = factorToBase
    };

    private static UnitDefinition LinearWeight(string symbol, string name, decimal factorToBase) => new()
    {
        Symbol = symbol,
        DisplayName = name,
        Category = MeasurementCategory.Weight,
        Kind = ConversionKind.Linear,
        FactorToBase = factorToBase
    };

    private static UnitDefinition Affine(string symbol, string name, decimal multiply, decimal add) => new()
    {
        Symbol = symbol,
        DisplayName = name,
        Category = MeasurementCategory.Temperature,
        Kind = ConversionKind.Affine,
        ToCelsiusMultiply = multiply,
        ToCelsiusAdd = add
    };

    // List APIs: length/weight by factorToBase (smallest first); temperature by fixed display order.
    private static int CompareWithinCategory(UnitDefinition a, UnitDefinition b)
    {
        if (a.Category != b.Category)
        {
            return a.Category.CompareTo(b.Category);
        }

        return a.Category switch
        {
            MeasurementCategory.Length or MeasurementCategory.Weight =>
                (a.FactorToBase ?? 0m).CompareTo(b.FactorToBase ?? 0m),
            MeasurementCategory.Temperature =>
                TemperatureDisplayOrder(a.Symbol).CompareTo(TemperatureDisplayOrder(b.Symbol)),
            _ => string.Compare(a.Symbol, b.Symbol, StringComparison.OrdinalIgnoreCase)
        };
    }

    private static int TemperatureDisplayOrder(string symbol) =>
        symbol.ToLowerInvariant() switch
        {
            "celsius" => 0,
            "fahrenheit" => 1,
            "kelvin" => 2,
            _ => 99
        };
}