using Autofac;
using Nameless.DependencyInjection.Autofac;

namespace Nameless.Environment {
    /// <summary>
    /// The Environment service registration.
    /// </summary>
    public sealed class EnvironmentModule : ModuleBase {

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IHostingEnvironment"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType HostingEnvironmentLifetimeScope { get; set; } = LifetimeScopeType.Singleton;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<HostingEnvironment> ()
                .As<IHostingEnvironment> ()
                .SetLifetimeScope (HostingEnvironmentLifetimeScope);

            base.Load (builder);
        }

        #endregion
    }
}