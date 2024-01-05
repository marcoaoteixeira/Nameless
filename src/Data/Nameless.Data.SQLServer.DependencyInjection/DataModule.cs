using Autofac;
using Nameless.Autofac;

namespace Nameless.Data.SQLServer.DependencyInjection {
    public sealed class DataModule : ModuleBase {
        #region Private Constants

        private const string DB_CONNECTION_MANAGER_TOKEN = $"{nameof(DbConnectionManager)}::5f162895-ef20-43be-8e9f-f00241f8aa62";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ResolveDbConnectionManager)
                .Named<IDbConnectionManager>(DB_CONNECTION_MANAGER_TOKEN)
                .SingleInstance();

            builder
                .Register(ResolveDatabase)
                .As<IDatabase>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IDbConnectionManager ResolveDbConnectionManager(IComponentContext ctx) {
            var sqlServerOptions = GetOptionsFromContext<SQLServerOptions>(ctx)
                ?? SQLServerOptions.Default;
            var logger = GetLoggerFromContext<DbConnectionManager>(ctx);
            var result = new DbConnectionManager(sqlServerOptions, logger);

            return result;
        }

        private static IDatabase ResolveDatabase(IComponentContext ctx) {
            var dbConnectionManager = ctx.ResolveNamed<IDbConnectionManager>(
                DB_CONNECTION_MANAGER_TOKEN
            );
            var dbConnection = dbConnectionManager.GetDbConnection();
            var logger = GetLoggerFromContext<Database>(ctx);
            var result = new Database(dbConnection, logger);

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