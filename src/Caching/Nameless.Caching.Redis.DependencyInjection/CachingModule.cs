using Autofac;
using Nameless.Autofac;
using Nameless.Caching.Redis.Impl;
using Nameless.Caching.Redis.Options;

namespace Nameless.Caching.Redis.DependencyInjection {
    public sealed class CachingModule : ModuleBase {
        #region Private Constants

        private const string CONFIGURATION_OPTIONS_FACTORY_TOKEN = $"{nameof(ConfigurationOptionsFactory)}::cb35ebee-c898-43de-8c3b-abbe8e9c71b2";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder.Register(ConfigurationFactoryResolver)
                   .Named<IConfigurationOptionsFactory>(CONFIGURATION_OPTIONS_FACTORY_TOKEN)
                   .SingleInstance();

            builder.Register(CacheResolver)
                   .As<ICache>()
                   .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IConfigurationOptionsFactory ConfigurationFactoryResolver(IComponentContext ctx)
            => new ConfigurationOptionsFactory(options: ctx.GetPocoOptions<RedisOptions>(),
                                               logger: ctx.GetLogger<RedisCache>());

        private static ICache CacheResolver(IComponentContext ctx) {
            var configurationFactory = ctx.ResolveNamed<IConfigurationOptionsFactory>(CONFIGURATION_OPTIONS_FACTORY_TOKEN);
            var configurationOptions = configurationFactory.CreateConfigurationOptions();
            var result = new RedisCache(configurationOptions);

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
