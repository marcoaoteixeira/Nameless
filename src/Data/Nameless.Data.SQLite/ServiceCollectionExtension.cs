using Microsoft.Extensions.DependencyInjection;
using Nameless.Data.SQLite.Options;

namespace Nameless.Data.SQLite {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection RegisterDatabaseService(this IServiceCollection self, Action<SQLiteOptions>? configure = null)
            => self.AddSingleton<IDatabaseService>(provider => {
                var options = provider.GetPocoOptions<SQLiteOptions>();

                configure?.Invoke(options);

                return new DatabaseService(
                    dbConnectionFactory: new DbConnectionFactory(options),
                    logger: provider.GetLogger<DatabaseService>()
                );
            });

        #endregion
    }
}
