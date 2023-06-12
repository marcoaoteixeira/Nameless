using System.Reflection;
using Autofac;
using Autofac.Core;
using Nameless.Autofac;

namespace Nameless.Data {

    /// <summary>
    /// The data common service registration.
    /// </summary>
    public sealed class DataModule : ModuleBase {

        #region Private Constants

        private const string DB_CONNECTION_FACTORY_KEY = "7e99f8b1-05ad-4a89-8e36-46a7660bc8a8";

        #endregion

        #region Public Properties

        public Type? DbConnectionFactoryImplementation { get; set; }

        #endregion

        #region Public Constructors

        public DataModule(params Assembly[] supportAssemblies)
            : base (supportAssemblies) { }

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder) {
            var dbConnectionFactoryImplementation = DbConnectionFactoryImplementation
                ?? SearchForImplementation<IDbConnectionFactory>()
                ?? throw new ImplementationNotFoundException(typeof(IDbConnectionFactory));
            builder
                .RegisterType(dbConnectionFactoryImplementation)
                .Named<IDbConnectionFactory>(DB_CONNECTION_FACTORY_KEY)
                .SingleInstance();

            builder
                .RegisterType<Database>()
                .As<IDatabase>()
                .WithParameter(ResolvedParameter.ForNamed<IDbConnectionFactory>(DB_CONNECTION_FACTORY_KEY))
                .SetLifetimeScope(LifetimeScopeType.PerScope);

            base.Load(builder);
        }

        #endregion
    }
}
