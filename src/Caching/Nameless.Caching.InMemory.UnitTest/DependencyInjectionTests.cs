using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Caching.InMemory;

public class DependencyInjectionTests {
    [Test]
    public void Register_And_Resolve_Service() {
        // arrange
        var services = new ServiceCollection();
        services.RegisterCache();

        using var provider = services.BuildServiceProvider();

        // act
        var sut = provider.GetRequiredService<ICache>();

        // assert
        Assert.That(sut, Is.InstanceOf<InMemoryCache>());
    }
}