using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.Validation.FluentValidation;

public class DependencyInjectionTests {
    [Fact]
    public void Register_Resolve_Service() {
        // arrange
        var services = new ServiceCollection();
        services.AddTransient<ILogger<ValidationService>>(_ => NullLogger<ValidationService>.Instance);
        services.RegisterValidationServices();
        using var container = services.BuildServiceProvider();

        // act
        var sut = container.GetService<IValidationService>();

        // assert
        Assert.That(sut, Is.InstanceOf<ValidationService>());
    }
}