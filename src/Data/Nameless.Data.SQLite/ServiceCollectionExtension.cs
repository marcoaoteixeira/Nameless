using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Data.Sqlite;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtension {
    public static IServiceCollection RegisterSqliteServices(this IServiceCollection self, Action<SqliteOptions> configure)
        => Prevent.Argument
                  .Null(self)
                  .Configure(configure)
                  .RegisterDatabaseServices();

    public static IServiceCollection RegisterSqliteServices(this IServiceCollection self, IConfigurationSection sqliteConfigSection)
        => Prevent.Argument
                  .Null(self)
                  .Configure<SqliteOptions>(sqliteConfigSection)
                  .RegisterDatabaseServices();

    private static IServiceCollection RegisterDatabaseServices(this IServiceCollection self)
        => self.AddSingleton<IDbConnectionFactory, DbConnectionFactory>()
               .AddSingleton<IDatabase, Database>();
}