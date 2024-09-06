using Microsoft.Extensions.DependencyInjection;
using Nameless.Data.SQLite.Options;

namespace Nameless.Data.SQLite;

public static class ServiceCollectionExtension {
    #region Public Static Methods

    public static IServiceCollection RegisterDatabaseService(this IServiceCollection self, Action<SQLiteOptions>? configure = null)
        => self.AddSingleton<IDatabaseService>(provider => {
            var options = provider.GetOptions<SQLiteOptions>();

            configure?.Invoke(options.Value);

            return new DatabaseService(
                dbConnectionFactory: new DbConnectionFactory(options.Value),
                logger: provider.GetLogger<DatabaseService>()
            );
        });

    #endregion
}