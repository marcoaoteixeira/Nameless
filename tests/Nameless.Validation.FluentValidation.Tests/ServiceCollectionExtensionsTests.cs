using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nameless.Testing.Tools;

namespace Nameless.Validation.FluentValidation;

public class ServiceCollectionExtensionsTests {
    [Fact]
    public void WhenRegisterServices_ThenResolveSuccessfully() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(Quick.Mock<ILogger<ValidationService>>());
        services.RegisterValidation(
            registration: _ => { }
        );

        using var container = services.BuildServiceProvider();

        // act
        var sut = container.GetService<IValidationService>();

        // assert
        Assert.IsType<ValidationService>(sut);
    }

    [Fact]
    public void WhenRegisterServices_IgnoreOpenGenericValidators_ThenResolveSuccessfully() {
        // arrange
        var services = new ServiceCollection();
        services.RegisterValidation(opts => {
            opts.IncludeAssemblies(typeof(ServiceCollectionExtensionsTests).Assembly);
        });

        using var container = services.BuildServiceProvider();

        // act
        var actual = container.GetServices<IValidator>();

        // assert
        Assert.DoesNotContain(actual, item => typeof(OpenGenericValidator<>).IsAssignableFrom(item.GetType()));
    }

    public class OpenGenericValidator<T> : AbstractValidator<T>;
}