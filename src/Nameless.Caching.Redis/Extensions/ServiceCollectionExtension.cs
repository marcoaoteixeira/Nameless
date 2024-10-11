using Microsoft.Extensions.DependencyInjection;
using Nameless.Caching.Redis.Impl;
using Nameless.Caching.Redis.Options;

namespace Nameless.Caching.Redis;

/// <summary>
/// Extension methods to register <see cref="ICache"/> for Redis.
/// </summary>
public static class ServiceCollectionExtension {
    /// <summary>
    /// Registers Redis as the current <see cref="ICache"/>.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/> instance.</param>
    /// <param name="configure">Configuration action.</param>
    /// <returns>Returns the current <see cref="IServiceCollection"/> so other configuration actions can be chained.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection AddRedisCache(this IServiceCollection self, Action<RedisOptions>? configure = null)
        => Prevent.Argument
                .Null(self, nameof(self))
                .AddSingleton<ICache>(provider => {
                    var options = provider.GetOptions<RedisOptions>();

                    configure?.Invoke(options.Value);

                    var configurationOptionsFactory = new ConfigurationOptionsFactory(
                        options: options,
                        logger: provider.GetLogger<ConfigurationOptionsFactory>()
                    );

                    return CreateRedisCache(provider, configurationOptionsFactory);
                });

    private static RedisCache CreateRedisCache(IServiceProvider provider, ConfigurationOptionsFactory configurationOptionsFactory)
        => new(configurationOptions: configurationOptionsFactory.CreateConfigurationOptions(),
               logger: provider.GetLogger<RedisCache>());
}