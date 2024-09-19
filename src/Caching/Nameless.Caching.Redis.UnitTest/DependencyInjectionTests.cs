using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Caching.Redis;

public class DependencyInjectionTests {
    [Test]
    public void Register_And_Resolve_Service() {
        // arrange
        var services = new ServiceCollection();
        services.AddRedisCache();
        using var provider = services.BuildServiceProvider();

        // act
        var sut = provider.GetRequiredService<ICache>();

        // assert
        Assert.That(sut, Is.InstanceOf<RedisCache>());
    }
}