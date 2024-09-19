using Microsoft.Extensions.DependencyInjection;
using Nameless.Data.SQLite.Options;

namespace Nameless.Data.SQLite;

public static class ServiceCollectionExtension {
    public static IServiceCollection AddSQLite(this IServiceCollection self, Action<SQLiteOptions>? configure = null)
        => Prevent.Argument
                  .Null(self)
                  .AddSingleton<IDatabaseService>(provider => {
                      var options = provider.GetOptions<SQLiteOptions>();

                      configure?.Invoke(options.Value);

                      return new DatabaseService(dbConnectionFactory: new DbConnectionFactory(options.Value),
                                                 logger: provider.GetLogger<DatabaseService>());
                  });
}