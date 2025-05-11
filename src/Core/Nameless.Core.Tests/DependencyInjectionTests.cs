using Microsoft.Extensions.DependencyInjection;
using Nameless.Services;

namespace Nameless;

public class DependencyInjectionTests {
    [Test]
    public void Register_And_Resolve_Services() {
        // arrange
        var services = new ServiceCollection();
        services.RegisterClock();

        using var provider = services.BuildServiceProvider();

        // act
        var clockService = provider.GetService<IClock>();

        // assert
        Assert.That(clockService, Is.InstanceOf<Clock>());
    }
}