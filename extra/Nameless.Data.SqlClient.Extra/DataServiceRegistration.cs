using Autofac;
using Nameless.IoC.Autofac;

namespace Nameless.Data.SqlClient {
    /// <summary>
    /// The data common service registration.
    /// </summary>
    public sealed class DataServiceRegistration : ServiceRegistrationBase {

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IDatabase"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Transient"/>.</remarks>
        public LifetimeScopeType DatabaseLifetimeScope { get; set; } = LifetimeScopeType.Transient;

        /// <summary>
        /// Gets or sets the <see cref="IDbConnectionFactory"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType DbConnectionFactoryLifetimeScope { get; set; } = LifetimeScopeType.Singleton;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<Database> ()
                .As<IDatabase> ()
                .SetLifetimeScope (DatabaseLifetimeScope);

            builder
                .RegisterType<DbConnectionFactory> ()
                .As<IDbConnectionFactory> ()
                .SetLifetimeScope (DbConnectionFactoryLifetimeScope);

            base.Load (builder);
        }

        #endregion
    }
}