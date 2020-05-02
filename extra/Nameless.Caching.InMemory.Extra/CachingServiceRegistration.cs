using Autofac;
using Nameless.IoC.Autofac;

namespace Nameless.Caching.InMemory {
    /// <summary>
    /// The Caching service registration.
    /// </summary>
    public sealed class CachingServiceRegistration : ServiceRegistrationBase {

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="ICache"/> <see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType MemoryCacheLifetimeScope { get; set; } = LifetimeScopeType.Singleton;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<MemoryCache> ()
                .As<ICache> ()
                .SetLifetimeScope (MemoryCacheLifetimeScope);

            base.Load (builder);
        }

        #endregion
    }
}