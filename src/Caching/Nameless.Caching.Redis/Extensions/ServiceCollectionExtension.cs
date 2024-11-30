using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Caching.Redis.Options;

namespace Nameless.Caching.Redis;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtension {
    /// <summary>
    /// Adds <see cref="ICache"/> service for Redis.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/> instance.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/> instance so other actions can be chained.
    /// </returns>
    public static IServiceCollection AddRedisCache(this IServiceCollection self, Action<RedisOptions> configure)
        => Prevent.Argument
                  .Null(self)
                  .Configure(configure)
                  .RegisterCachingServices();

    /// <summary>
    /// Adds <see cref="ICache"/> service for Redis.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/> instance.</param>
    /// <param name="redisConfigSection">The configuration section for Redis options.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/> instance so other actions can be chained.
    /// </returns>
    public static IServiceCollection AddRedisCache(this IServiceCollection self, IConfigurationSection redisConfigSection)
        => Prevent.Argument
                  .Null(self)
                  .Configure<RedisOptions>(redisConfigSection)
                  .RegisterCachingServices();

    private static IServiceCollection RegisterCachingServices(this IServiceCollection services)
        => services.AddSingleton<IConfigurationOptionsFactory, ConfigurationOptionsFactory>()
                   .AddSingleton<IConnectionMultiplexerManager, ConnectionMultiplexerManager>()
                   .AddSingleton<ICache, RedisCache>();
}