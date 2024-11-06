using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Data.SQLite.Options;

namespace Nameless.Data.SQLite;

public static class ServiceCollectionExtension {
    public static IServiceCollection AddSQLiteDatabase(this IServiceCollection self, Action<SQLiteOptions> configure)
        => Prevent.Argument
                  .Null(self)
                  .Configure(configure)
                  .RegisterDatabaseServices();

    public static IServiceCollection AddSQLiteDatabase(this IServiceCollection self, IConfigurationSection sqliteOptionsConfigurationSection)
        => Prevent.Argument
                  .Null(self)
                  .Configure<SQLiteOptions>(sqliteOptionsConfigurationSection)
                  .RegisterDatabaseServices();

    private static IServiceCollection RegisterDatabaseServices(this IServiceCollection self)
        => self.AddSingleton<IDbConnectionFactory, DbConnectionFactory>()
               .AddSingleton<IDatabase, Database>();
}