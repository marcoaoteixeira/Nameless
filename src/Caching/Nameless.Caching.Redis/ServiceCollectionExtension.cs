using Microsoft.Extensions.DependencyInjection;
using Nameless.Caching.Redis.Impl;
using Nameless.Caching.Redis.Options;

namespace Nameless.Caching.Redis {
    /// <summary>
    /// Extension methods to register <see cref="ICacheService"/> for Redis.
    /// </summary>
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        /// <summary>
        /// Registers Redis as the current <see cref="ICacheService"/>.
        /// </summary>
        /// <param name="self">The current <see cref="IServiceCollection"/> instance.</param>
        /// <param name="configure">Configuration action.</param>
        /// <returns>Returns the current <see cref="IServiceCollection"/> so other configuration actions can be chained.</returns>
        public static IServiceCollection RegisterCacheService(this IServiceCollection self, Action<RedisOptions>? configure = null)
            => self.AddSingleton<ICacheService>(provider => {
                var options = provider.GetOptions<RedisOptions>();

                configure?.Invoke(options.Value);

                var configurationOptionsFactory = new ConfigurationOptionsFactory(
                    options: options.Value,
                    logger: provider.GetLogger<ConfigurationOptionsFactory>()
                );

                return new RedisCacheService(
                    configurationOptions: configurationOptionsFactory.CreateConfigurationOptions(),
                    logger: provider.GetLogger<RedisCacheService>()
                );
            });

        #endregion
    }
}
