using UnitConversion.Domain.Entities;
using UnitConversion.Domain.Enums;

namespace UnitConversion.Application.Interfaces;

public interface IUnitCatalog
{
    bool TryGet(string symbol, out UnitDefinition unit);

    IReadOnlyList<UnitDefinition> GetAll();

    IReadOnlyList<UnitDefinition> GetByCategory(MeasurementCategory category);

    IReadOnlyList<MeasurementCategory> GetCategories();
}