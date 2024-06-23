using Microsoft.Extensions.DependencyInjection;
using Nameless.Data.SQLServer.Options;

namespace Nameless.Data.SQLServer {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection RegisterDatabaseService(this IServiceCollection self, Action<SQLServerOptions>? configure = null)
            => self.AddSingleton<IDatabaseService>(provider => {
                var options = provider.GetPocoOptions<SQLServerOptions>();

                configure?.Invoke(options);

                return new DatabaseService(
                    dbConnectionFactory: new DbConnectionFactory(options),
                    logger: provider.GetLogger<DatabaseService>()
                );
            });

        #endregion
    }
}
