using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UnitConversion.Application.Factories;
using UnitConversion.Application.Interfaces;
using UnitConversion.Application.Services;
using UnitConversion.Application.Strategies;
using UnitConversion.Application.Validators;

namespace UnitConversion.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Strategy + factory + scoped use case; catalog comes from Infrastructure.
        services.AddSingleton<IConversionStrategy, LengthConversionStrategy>();
        services.AddSingleton<IConversionStrategy, WeightConversionStrategy>();
        services.AddSingleton<IConversionStrategy, TemperatureConversionStrategy>();
        services.AddSingleton<ConversionStrategyFactory>();

        services.AddScoped<UnitConversionService>();
        services.AddSingleton<UnitCatalogService>();

        services.AddValidatorsFromAssemblyContaining<ConversionRequestValidator>();

        return services;
    }
}