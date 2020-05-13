using Autofac;

namespace Nameless.DependencyInjection.Autofac {
    /// <summary>
    /// The Services service registration.
    /// </summary>
    public sealed class DependencyInjectionModule : ModuleBase {

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IServiceResolver"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Transient"/>.</remarks>
        public LifetimeScopeType ServiceResolverLifetimeScope { get; set; } = LifetimeScopeType.Transient;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .Register (ctx => new ServiceResolver (ctx.Resolve<ILifetimeScope> (), isRoot: true))
                .As<IServiceResolver> ()
                .SetLifetimeScope (ServiceResolverLifetimeScope);

            base.Load (builder);
        }

        #endregion
    }
}