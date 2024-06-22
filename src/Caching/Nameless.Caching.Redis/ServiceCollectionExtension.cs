using Microsoft.Extensions.DependencyInjection;
using Nameless.Caching.Redis.Impl;
using Nameless.Caching.Redis.Options;

namespace Nameless.Caching.Redis {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection RegisterCaching(this IServiceCollection self)
            => self.AddSingleton<ICacheService>(provider => {
                var configurationOptionsFactory = new ConfigurationOptionsFactory(
                    options: provider.GetPocoOptions<RedisOptions>(),
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
