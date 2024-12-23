using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Data.SQLServer.Options;

namespace Nameless.Data.SQLServer;

public static class ServiceCollectionExtension {
    public static IServiceCollection RegisterSqlServerDatabase(this IServiceCollection self, Action<SQLServerOptions> configure)
        => Prevent.Argument
                  .Null(self)
                  .Configure(configure)
                  .RegisterDatabaseServices();

    public static IServiceCollection RegisterSqlServerDatabase(this IServiceCollection self, IConfigurationSection sqlServerConfigSection)
        => Prevent.Argument
                  .Null(self)
                  .Configure<SQLServerOptions>(sqlServerConfigSection)
                  .RegisterDatabaseServices();

    private static IServiceCollection RegisterDatabaseServices(this IServiceCollection self)
        => self.AddSingleton<IDbConnectionFactory, DbConnectionFactory>()
               .AddSingleton<IDatabase, Database>();
}