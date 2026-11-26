using UnitConversion.Application.Interfaces;
using UnitConversion.Domain.Enums;

namespace UnitConversion.Application.Factories;

public sealed class ConversionStrategyFactory
{
    // One IConversionStrategy implementation per MeasurementCategory (registered in DI).
    private readonly IReadOnlyDictionary<MeasurementCategory, IConversionStrategy> _strategies;

    public ConversionStrategyFactory(IEnumerable<IConversionStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.Category);
    }

    public IConversionStrategy Get(MeasurementCategory category)
    {
        if (!_strategies.TryGetValue(category, out var strategy))
        {
            throw new InvalidOperationException($"No conversion strategy registered for category {category}.");
        }

        return strategy;
    }
}