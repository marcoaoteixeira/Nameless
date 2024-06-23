using Microsoft.Extensions.DependencyInjection;
using Nameless.Caching.Redis.Impl;
using Nameless.Caching.Redis.Options;

namespace Nameless.Caching.Redis {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection RegisterCacheService(this IServiceCollection self, Action<RedisOptions>? configure = null)
            => self.AddSingleton<ICacheService>(provider => {
                var options = provider.GetPocoOptions<RedisOptions>();

                configure?.Invoke(options);

                var configurationOptionsFactory = new ConfigurationOptionsFactory(
                    options: options,
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
