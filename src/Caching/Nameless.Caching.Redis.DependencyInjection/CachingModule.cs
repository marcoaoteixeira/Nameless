using Autofac;
using Nameless.Autofac;
using Nameless.Caching.Redis.Impl;
using Nameless.Caching.Redis.Options;

namespace Nameless.Caching.Redis.DependencyInjection {
    public sealed class CachingModule : ModuleBase {
        #region Private Constants

        private const string CONFIGURATION_FACTORY_TOKEN = $"{nameof(IConfigurationFactory)}::cb35ebee-c898-43de-8c3b-abbe8e9c71b2";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ConfigurationFactoryResolver)
                .As<IConfigurationFactory>()
                .SingleInstance();

            builder
                .Register(CacheResolver)
                .As<ICache>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IConfigurationFactory ConfigurationFactoryResolver(IComponentContext ctx) {
            var result = new ConfigurationFactory(
                options: ctx.GetOptions<RedisOptions>(),
                logger: ctx.GetLogger<RedisCache>()
            );

            return result;
        }

        private static ICache CacheResolver(IComponentContext ctx) {
            var configurationFactory = ctx.ResolveNamed<IConfigurationFactory>(CONFIGURATION_FACTORY_TOKEN);
            var result = new RedisCache(configurationFactory);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static ContainerBuilder RegisterCachingModule(this ContainerBuilder self) {
            self.RegisterModule<CachingModule>();

            return self;
        }

        #endregion
    }
}
