using Autofac;
using Nameless.IoC.Autofac;

namespace Nameless.IoC {
    /// <summary>
    /// The Services service registration.
    /// </summary>
    public sealed class IoCServiceRegistration : ServiceRegistrationBase {

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
                .RegisterType<ServiceResolver> ()
                .As<IServiceResolver> ()
                .SetLifetimeScope (ServiceResolverLifetimeScope);

            base.Load (builder);
        }

        #endregion
    }
}