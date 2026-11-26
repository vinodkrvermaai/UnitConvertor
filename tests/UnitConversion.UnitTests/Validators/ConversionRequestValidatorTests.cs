using FluentValidation.TestHelper;
using UnitConversion.Application.DTOs;
using UnitConversion.Application.Validators;
using UnitConversion.Infrastructure.Data;

namespace UnitConversion.UnitTests.Validators;

[TestFixture]
public sealed class ConversionRequestValidatorTests
{
    private ConversionRequestValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new ConversionRequestValidator(new HardcodedUnitCatalog());

    [Test]
    public void Should_HaveError_When_FromIsEmpty()
    {
        var result = _validator.TestValidate(new ConversionRequest { Value = 1, From = "", To = "meter" });
        result.ShouldHaveValidationErrorFor(x => x.From);
    }

    [Test]
    public void Should_HaveError_When_UnknownFromUnit()
    {
        var result = _validator.TestValidate(new ConversionRequest { Value = 1, From = "xyz", To = "meter" });
        result.ShouldHaveValidationErrorFor(x => x.From);
    }

    [Test]
    public void Should_HaveError_When_CrossCategory()
    {
        var result = _validator.TestValidate(new ConversionRequest { Value = 1, From = "meter", To = "kilogram" });
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Test]
    public void Should_HaveError_When_FromEqualsTo()
    {
        var result = _validator.TestValidate(new ConversionRequest { Value = 1, From = "meter", To = "meter" });
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Test]
    public void Should_NotHaveErrors_When_ValidLengthConversion()
    {
        var result = _validator.TestValidate(new ConversionRequest { Value = 10, From = "meter", To = "foot" });
        result.ShouldNotHaveAnyValidationErrors();
    }
}