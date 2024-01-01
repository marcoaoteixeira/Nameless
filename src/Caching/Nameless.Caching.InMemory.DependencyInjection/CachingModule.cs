using Autofac;
using Microsoft.Extensions.Caching.Memory;
using Nameless.Autofac;

namespace Nameless.Caching.InMemory.DependencyInjection {
    public sealed class CachingModule : ModuleBase {
        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ResolveCache)
                .As<ICache>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods}

        private static ICache ResolveCache(IComponentContext ctx) {
            var memoryCacheOptions = GetOptionsFromContext<MemoryCacheOptions>(ctx)
                ?? new MemoryCacheOptions();
            var memoryCache = new MemoryCache(memoryCacheOptions);
            var result = new InMemoryCache(memoryCache);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static ContainerBuilder AddCaching(this ContainerBuilder self) {
            self.RegisterModule<CachingModule>();

            return self;
        }

        #endregion
    }
}
