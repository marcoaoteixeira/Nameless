using System.Reflection;
using Autofac;
using Nameless.Autofac;

namespace Nameless.Data {
    /// <summary>
    /// The data common service registration.
    /// </summary>
    public sealed class DataModule : ModuleBase {
        #region Private Constants

        private const string DB_CONNECTION_FACTORY_TOKEN = "DbConnectionFactory.7e99f8b1-05ad-4a89-8e36-46a7660bc8a8";

        #endregion

        #region Public Properties

        public Type? DbConnectionFactoryImplementation { get; set; }

        #endregion

        #region Public Constructors

        public DataModule(params Assembly[] supportAssemblies)
            : base(supportAssemblies) { }

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder) {
            var dbConnectionFactoryImplementation = DbConnectionFactoryImplementation
                ?? SearchForImplementation<IDbConnectionFactory>()
                ?? throw new InvalidOperationException($"Could not find implementation for {nameof(IDbConnectionFactory)}.");

            builder
                .RegisterType(dbConnectionFactoryImplementation)
                .Named<IDbConnectionFactory>(DB_CONNECTION_FACTORY_TOKEN)
                .SingleInstance();

            builder
                .Register(DatabaseResolver)
                .As<IDatabase>()
                .InstancePerDependency();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IDatabase DatabaseResolver(IComponentContext context) {
            var factory = context.ResolveNamed<IDbConnectionFactory>(DB_CONNECTION_FACTORY_TOKEN);
            var database = new Database(factory);

            return database;
        }

        #endregion
    }
}
