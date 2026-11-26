using FluentValidation;
using UnitConversion.Application.DTOs;
using UnitConversion.Application.Interfaces;

namespace UnitConversion.Application.Validators;

public sealed class ConversionRequestValidator : AbstractValidator<ConversionRequest>
{
    public ConversionRequestValidator(IUnitCatalog catalog)
    {
        // HTTP 400 for unknown or cross-category units is enforced here only.
        RuleFor(x => x.From)
            .NotEmpty().WithMessage("Source unit 'from' is required.")
            .MaximumLength(32);

        RuleFor(x => x.To)
            .NotEmpty().WithMessage("Target unit 'to' is required.")
            .MaximumLength(32);

        RuleFor(x => x.From)
            .Must(symbol => catalog.TryGet(symbol, out _))
            .WithMessage("Unknown source unit '{PropertyValue}'. Use GET /api/v1/units for supported symbols.")
            .When(x => !string.IsNullOrWhiteSpace(x.From));

        RuleFor(x => x.To)
            .Must(symbol => catalog.TryGet(symbol, out _))
            .WithMessage("Unknown target unit '{PropertyValue}'. Use GET /api/v1/units for supported symbols.")
            .When(x => !string.IsNullOrWhiteSpace(x.To));

        RuleFor(x => x)
            .Must(x => !string.Equals(x.From?.Trim(), x.To?.Trim(), StringComparison.OrdinalIgnoreCase))
            .WithMessage("Source and target units must differ.")
            .When(x => !string.IsNullOrWhiteSpace(x.From) && !string.IsNullOrWhiteSpace(x.To));

        RuleFor(x => x)
            .Must(x =>
            {
                if (!catalog.TryGet(x.From, out var from) || !catalog.TryGet(x.To, out var to))
                {
                    return true;
                }

                return from.Category == to.Category;
            })
            .WithMessage(x => $"Cannot convert from '{x.From}' to '{x.To}' because they belong to different categories.")
            .When(x => !string.IsNullOrWhiteSpace(x.From) && !string.IsNullOrWhiteSpace(x.To)
                && !string.Equals(x.From.Trim(), x.To.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}