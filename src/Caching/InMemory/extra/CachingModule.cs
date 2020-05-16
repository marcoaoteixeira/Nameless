using Autofac;
using Nameless.DependencyInjection.Autofac;

namespace Nameless.Caching.InMemory {
    /// <summary>
    /// The Caching service registration.
    /// </summary>
    public sealed class CachingModule : ModuleBase {

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="ICache"/> <see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType CacheLifetimeScope { get; set; } = LifetimeScopeType.Singleton;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<InMemoryCache> ()
                .As<ICache> ()
                .SetLifetimeScope (CacheLifetimeScope);

            base.Load (builder);
        }

        #endregion
    }
}