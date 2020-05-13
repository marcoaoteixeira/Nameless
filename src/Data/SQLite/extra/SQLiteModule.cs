using Autofac;
using Autofac.Core;
using Nameless.DependencyInjection.Autofac;

namespace Nameless.Data.SQLite {
    /// <summary>
    /// The data common service registration.
    /// </summary>
    public sealed class SQLiteModule : ModuleBase {

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
                .WithParameter (new ResolvedParameter (
                    predicate: (param, ctx) => param.ParameterType == typeof (string),
                    valueAccessor: (param, ctx) => (ctx.ResolveOptional<DatabaseSettings> () ?? new DatabaseSettings ()).ConnectionString
                ))
                .SetLifetimeScope (DbConnectionFactoryLifetimeScope);

            base.Load (builder);
        }

        #endregion
    }
}