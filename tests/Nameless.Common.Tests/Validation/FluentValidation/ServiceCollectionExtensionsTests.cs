using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Validation.FluentValidation;

public record ValidatedThing(string Name);

public class ValidatedThingValidator : AbstractValidator<ValidatedThing> {
    public ValidatedThingValidator() {
        RuleFor(x => x.Name).NotEmpty();
    }
}

[UnitTest]
public class ServiceCollectionExtensionsTests {
    [Fact]
    public void WithFluentValidationValidator_Generic_AddsType() {
        // arrange
        var sut = new FluentValidationValidatorRegistration().WithUseAssemblyScan(false);

        // act
        var returned = sut.WithFluentValidationValidator<ValidatedThingValidator>();

        // assert
        Assert.Multiple(
            () => Assert.Same(sut, returned),
            () => Assert.Contains(typeof(ValidatedThingValidator), sut.Validators)
        );
    }

    [Fact]
    public void WithFluentValidationValidator_WithUnrelatedType_Throws() {
        // arrange
        var sut = new FluentValidationValidatorRegistration();

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.WithFluentValidationValidator(typeof(string)));
    }

    [Fact]
    public void WithFluentValidationValidator_WithAbstractType_Throws() {
        // arrange
        var sut = new FluentValidationValidatorRegistration();

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.WithFluentValidationValidator(typeof(AbstractValidator<ValidatedThing>)));
    }

    [Fact]
    public async Task RegisterValidator_RegistersValidatorThatUsesRegisteredFluentValidators() {
        // arrange
        var services = new ServiceCollection();

        // act
        var returned = services.RegisterValidator(r => r.WithUseAssemblyScan(false).WithFluentValidationValidator<ValidatedThingValidator>());
        using var provider = services.BuildServiceProvider();

        var validator = provider.GetRequiredService<IValidator>();
        var result = await validator.ValidateAsync(new ValidatedThing(""), new Dictionary<string, object>(), TestContext.Current.CancellationToken);

        // assert
        Assert.Multiple(
            () => Assert.Same(services, returned),
            () => Assert.IsType<FluentValidationValidator>(validator),
            () => Assert.False(result.Success)
        );
    }

    [Fact]
    public void RegisterValidator_WithoutConfigure_DoesNotThrow() {
        // arrange
        var services = new ServiceCollection();

        // act
        var exception = Record.Exception(() => services.RegisterValidator());

        // assert
        Assert.Null(exception);
    }
}
