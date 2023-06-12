using Autofac;
using Nameless.Autofac;

namespace Nameless.Caching.InMemory {

    public sealed class CachingModule : ModuleBase {

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .RegisterType<InMemoryCache>()
                .As<ICache>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion
    }
}
