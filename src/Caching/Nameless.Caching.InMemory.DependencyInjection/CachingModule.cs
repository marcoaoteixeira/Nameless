using Autofac;
using Microsoft.Extensions.Caching.Memory;
using Nameless.Autofac;

namespace Nameless.Caching.InMemory.DependencyInjection {
    public sealed class CachingModule : ModuleBase {
        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(CacheResolver)
                .As<ICache>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static ICache CacheResolver(IComponentContext ctx) {
            var memoryCacheOptions = GetOptionsFromContext<MemoryCacheOptions>(ctx)
                ?? new();
            var result = new InMemoryCache(memoryCacheOptions);

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
