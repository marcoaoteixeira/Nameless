using System.Reflection;
using Autofac;
using Autofac.Core.Registration;
using Microsoft.Extensions.Configuration;
using Nameless.Autofac;
using CoreRoot = Nameless.Root;

namespace Nameless.Data.SQLServer.DependencyInjection {
    public sealed class DataModule : ModuleBase {
        #region Private Constants

        private const string DB_CONNECTION_MANAGER_TOKEN = $"{nameof(DbConnectionManager)}::5f162895-ef20-43be-8e9f-f00241f8aa62";

        #endregion

        #region Public Constructors

        public DataModule()
            : base([]) { }

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ResolveDatabase)
                .As<IDatabase>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static SQLServerOptions? GetSQLServerOptions(IComponentContext ctx) {
            var configuration = ctx.ResolveOptional<IConfiguration>();
            var options = configuration?
                .GetSection(nameof(SQLServerOptions).RemoveTail(CoreRoot.Defaults.OptsSetsTails))
                .Get<SQLServerOptions>();

            return options;
        }

        private static IDatabase ResolveDatabase(IComponentContext ctx) {
            var sqlServerOptions = GetSQLServerOptions(ctx);
            var dbConnectionManager = new DbConnectionManager(sqlServerOptions);
            var connection = dbConnectionManager.GetDbConnection();
            var result = new Database(connection);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static IModuleRegistrar AddData(this ContainerBuilder self)
            => self.RegisterModule<DataModule>();

        #endregion
    }
}