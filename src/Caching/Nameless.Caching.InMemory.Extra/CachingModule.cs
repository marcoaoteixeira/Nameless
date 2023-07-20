using Autofac;
using Microsoft.Extensions.Caching.Memory;
using Nameless.Autofac;

namespace Nameless.Caching.InMemory {
    public sealed class CachingModule : ModuleBase {
        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(RegisterInMemoryCache)
                .As<ICache>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static ICache RegisterInMemoryCache(IComponentContext context) {
            var opts = context.ResolveOptional<MemoryCacheOptions>() ?? new();

            return new InMemoryCache(opts);
        }

        #endregion
    }
}
