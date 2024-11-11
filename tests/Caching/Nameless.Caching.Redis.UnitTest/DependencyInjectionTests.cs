using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Mockers;

namespace Nameless.Caching.Redis;

public class DependencyInjectionTests {
    [Test]
    public void WhenAddingRedisCache_WithConfigurationSectionForOptions_ThenResolveCacheService() {
        // arrange
        var services = new ServiceCollection();

        services.AddSingleton<ILogger<RedisCache>>(NullLogger<RedisCache>.Instance);
        services.AddSingleton<ILogger<ConfigurationOptionsFactory>>(NullLogger<ConfigurationOptionsFactory>.Instance);

        services.AddRedisCache(new ConfigurationSectionMocker().Build());

        using var provider = services.BuildServiceProvider();

        // act
        var sut = provider.GetRequiredService<ICache>();

        // assert
        Assert.That(sut, Is.InstanceOf<RedisCache>());
    }

    [Test]
    public void WhenAddingRedisCache_WithOptionsConfigureAction_ThenResolveCacheService() {
        // arrange
        var services = new ServiceCollection();

        services.AddSingleton<ILogger<RedisCache>>(NullLogger<RedisCache>.Instance);
        services.AddSingleton<ILogger<ConfigurationOptionsFactory>>(NullLogger<ConfigurationOptionsFactory>.Instance);

        services.AddRedisCache(_ => { });

        using var provider = services.BuildServiceProvider();

        // act
        var sut = provider.GetRequiredService<ICache>();

        // assert
        Assert.That(sut, Is.InstanceOf<RedisCache>());
    }
}