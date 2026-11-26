using UnitConversion.Application.DTOs;
using UnitConversion.Application.Interfaces;
using UnitConversion.Domain.Enums;

namespace UnitConversion.Application.Services;

public sealed class UnitCatalogService
{
    private readonly IUnitCatalog _catalog;

    public UnitCatalogService(IUnitCatalog catalog)
    {
        _catalog = catalog;
    }

    public IReadOnlyList<UnitResponse> GetUnits(MeasurementCategory? category = null)
    {
        var units = category is { } c ? _catalog.GetByCategory(c) : _catalog.GetAll();
        return units.Select(Map).ToList();
    }

    public IReadOnlyList<string> GetCategoryNames() =>
        _catalog.GetCategories().Select(c => c.ToString()).ToList();

    private static UnitResponse Map(Domain.Entities.UnitDefinition u) => new()
    {
        Symbol = u.Symbol,
        Name = u.DisplayName,
        Category = u.Category.ToString()
    };
}