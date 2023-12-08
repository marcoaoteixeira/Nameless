using System.Reflection;
using Autofac;
using Autofac.Core.Registration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Nameless.Autofac;
using CoreRoot = Nameless.Root;

namespace Nameless.Caching.InMemory.DependencyInjection {
    public sealed class CachingModule : ModuleBase {
        #region Private Constants

        private const string MEMORY_CACHE_TOKEN = $"{nameof(MemoryCache)}::e69a9cb8-f681-476f-935a-d9c2d634947c";

        #endregion

        #region Public Constructors

        public CachingModule()
            : base([]) { }

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ResolveCache)
                .As<ICache>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static MemoryCacheOptions GetMemoryCacheOptions(IComponentContext ctx) {
            var configuration = ctx.ResolveOptional<IConfiguration>();
            var options = configuration?
                .GetSection(nameof(MemoryCacheOptions).RemoveTail(CoreRoot.Defaults.OptsSetsTails))
                .Get<MemoryCacheOptions>();

            return options ?? new();
        }

        private static ICache ResolveCache(IComponentContext ctx) {
            var memoryCacheOptions = GetMemoryCacheOptions(ctx);
            var memoryCache = new MemoryCache(memoryCacheOptions);
            var result = new InMemoryCache(memoryCache);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static IModuleRegistrar AddCaching(this ContainerBuilder self)
            => self.RegisterModule<CachingModule>();

        #endregion
    }
}
