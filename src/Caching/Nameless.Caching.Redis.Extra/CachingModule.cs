using Autofac;
using Nameless.Autofac;
using StackExchange.Redis;

namespace Nameless.Caching.Redis {

    public sealed class CachingModule : ModuleBase {

        #region Private Constants

        private const string REDIS_DATABASE_MANAGER_KEY = "[RedisDatabaseManager] 6e88deea-b059-4ff5-bafb-3a8d36976294";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .RegisterType<RedisDatabaseManager>()
                .Named<IRedisDatabaseManager>(REDIS_DATABASE_MANAGER_KEY)
                .WithParameter(
                    parameterSelector: (param, ctx) => param.ParameterType == typeof(RedisOptions),
                    valueProvider: (param, ctx) => ctx.ResolveOptional<RedisOptions>() ?? RedisOptions.Default
                )
                .SingleInstance();

            builder
                .RegisterType<RedisCache>()
                .As<ICache>()
                .WithParameter(
                    parameterSelector: (param, ctx) => param.ParameterType == typeof(IDatabase),
                    valueProvider: (param, ctx) => ctx.ResolveNamed<IRedisDatabaseManager>(REDIS_DATABASE_MANAGER_KEY).GetDatabase()
                )
                .InstancePerDependency();

            base.Load(builder);
        }

        #endregion
    }
}
