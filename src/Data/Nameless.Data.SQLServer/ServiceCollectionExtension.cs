using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Data.SqlServer;

public static class ServiceCollectionExtension {
    public static IServiceCollection RegisterSqlServerServices(this IServiceCollection self, Action<SqlServerOptions> configure)
        => Prevent.Argument
                  .Null(self)
                  .Configure(configure)
                  .RegisterDatabaseServices();

    public static IServiceCollection RegisterSqlServerServices(this IServiceCollection self, IConfigurationSection sqlServerConfigSection)
        => Prevent.Argument
                  .Null(self)
                  .Configure<SqlServerOptions>(sqlServerConfigSection)
                  .RegisterDatabaseServices();

    private static IServiceCollection RegisterDatabaseServices(this IServiceCollection self)
        => self.AddSingleton<IDbConnectionFactory, DbConnectionFactory>()
               .AddSingleton<IDatabase, Database>();
}