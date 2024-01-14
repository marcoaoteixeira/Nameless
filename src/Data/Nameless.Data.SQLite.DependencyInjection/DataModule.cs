using Autofac;
using Nameless.Autofac;

namespace Nameless.Data.SQLite.DependencyInjection {
    public sealed class DataModule : ModuleBase {
        #region Private Constants

        private const string DB_CONNECTION_FACTORY_TOKEN = $"{nameof(DbConnectionFactory)}::a2ea46d6-b47e-4885-9fa1-ab7206ab3909";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(DbConnectionFactoryResolver)
                .Named<IDbConnectionFactory>(DB_CONNECTION_FACTORY_TOKEN)
                .SingleInstance();

            builder
                .Register(DatabaseResolver)
                .As<IDatabase>()
                .InstancePerLifetimeScope();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IDbConnectionFactory DbConnectionFactoryResolver(IComponentContext ctx) {
            var sqlServerOptions = GetOptionsFromContext<SQLiteOptions>(ctx)
                ?? SQLiteOptions.Default;
            var result = new DbConnectionFactory(sqlServerOptions);

            return result;
        }

        private static IDatabase DatabaseResolver(IComponentContext ctx) {
            var dbConnectionFactory = ctx.ResolveNamed<IDbConnectionFactory>(
                DB_CONNECTION_FACTORY_TOKEN
            );
            var logger = GetLoggerFromContext<Database>(ctx);
            var result = new Database(dbConnectionFactory, logger);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static ContainerBuilder RegisterDataModule(this ContainerBuilder self) {
            self.RegisterModule<DataModule>();

            return self;
        }

        #endregion
    }
}