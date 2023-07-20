using Autofac;
using Nameless.Autofac;

namespace Nameless.Caching.Redis {
    public sealed class CachingModule : ModuleBase {
        #region Private Constants

        private const string REDIS_DATABASE_MANAGER_TOKEN = "RedisDatabaseManager.6e88deea-b059-4ff5-bafb-3a8d36976294";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(RedisDatabaseManagerResolver)
                .Named<IRedisDatabaseManager>(REDIS_DATABASE_MANAGER_TOKEN)
                .SingleInstance();

            builder
                .Register(RedisCacheResolver)
                .As<ICache>()
                .InstancePerDependency();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IRedisDatabaseManager RedisDatabaseManagerResolver(IComponentContext context) {
            var options = context.ResolveOptional<RedisOptions>() ?? RedisOptions.Default;
            var manager = new RedisDatabaseManager(options);

            return manager;
        }

        private static ICache RedisCacheResolver(IComponentContext context) {
            var manager = context.ResolveNamed<IRedisDatabaseManager>(REDIS_DATABASE_MANAGER_TOKEN);
            var cache = new RedisCache(manager);

            return cache;
        }

        #endregion
    }
}
