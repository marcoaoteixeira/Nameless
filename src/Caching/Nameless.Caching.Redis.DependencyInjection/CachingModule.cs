using Autofac;
using Nameless.Autofac;
using Nameless.Caching.Redis.Impl;

namespace Nameless.Caching.Redis.DependencyInjection {
    public sealed class CachingModule : ModuleBase {
        #region Private Constants

        private const string CONNECTION_MULTIPLEXER_MANAGER_TOKEN = $"{nameof(ConnectionMultiplexerManager)}::ded40b71-3532-47ed-a3c9-9a5d76e37916";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ResolveConnectionMultiplexerManager)
                .Named<IConnectionMultiplexerManager>(CONNECTION_MULTIPLEXER_MANAGER_TOKEN)
                .SingleInstance();

            builder
                .Register(ResolveRedisCache)
                .As<ICache>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IConnectionMultiplexerManager ResolveConnectionMultiplexerManager(IComponentContext ctx) {
            var options = GetOptionsFromContext<RedisOptions>(ctx)
                ?? RedisOptions.Default;
            var logger = GetLoggerFromContext<ConnectionMultiplexerManager>(ctx);
            var result = new ConnectionMultiplexerManager(options, logger);

            return result;
        }

        private static ICache ResolveRedisCache(IComponentContext ctx) {
            var connectionMultiplexerManager = ctx.ResolveNamed<IConnectionMultiplexerManager>(CONNECTION_MULTIPLEXER_MANAGER_TOKEN);
            var multiplexer = connectionMultiplexerManager.GetMultiplexer();
            var database = multiplexer.GetDatabase();
            var result = new RedisCache(database);

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
