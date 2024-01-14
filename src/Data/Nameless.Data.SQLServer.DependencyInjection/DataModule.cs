using Autofac;
using Nameless.Autofac;

namespace Nameless.Data.SQLServer.DependencyInjection {
    public sealed class DataModule : ModuleBase {
        #region Private Constants

        private const string DB_CONNECTION_FACTORY_TOKEN = $"{nameof(DbConnectionFactory)}::5f162895-ef20-43be-8e9f-f00241f8aa62";

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
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IDbConnectionFactory DbConnectionFactoryResolver(IComponentContext ctx) {
            var sqlServerOptions = GetOptionsFromContext<SQLServerOptions>(ctx)
                ?? SQLServerOptions.Default;
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