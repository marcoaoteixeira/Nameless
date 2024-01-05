using Autofac;
using Microsoft.Extensions.Caching.Memory;
using Nameless.Autofac;

namespace Nameless.Caching.InMemory.DependencyInjection {
    public sealed class CachingModule : ModuleBase {
        #region Private Constants

        private const string MEMORY_CACHE_TOKEN = $"{nameof(MemoryCache)}::c041f63c-04a2-4e09-aed6-dd06db9fa188";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ResolveMemoryCache)
                .Named<IMemoryCache>(MEMORY_CACHE_TOKEN)
                .SingleInstance();

            builder
                .Register(ResolveCache)
                .As<ICache>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IMemoryCache ResolveMemoryCache(IComponentContext ctx) {
            var memoryCacheOptions = GetOptionsFromContext<MemoryCacheOptions>(ctx)
                ?? new MemoryCacheOptions();
            var result = new MemoryCache(memoryCacheOptions);

            return result;
        }

        private static ICache ResolveCache(IComponentContext ctx) {
            var memoryCache = ctx.ResolveNamed<IMemoryCache>(MEMORY_CACHE_TOKEN);
            var result = new InMemoryCache(memoryCache);

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
