using Microsoft.Extensions.DependencyInjection;
using UnitConversion.Application.Interfaces;
using UnitConversion.Infrastructure.Data;

namespace UnitConversion.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IUnitCatalog, HardcodedUnitCatalog>();
        return services;
    }
}