using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.Data.Sqlite;

/// <summary>
///     Extension methods for <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions {
    private const string DB_CONNECTION_FACTORY_KEY = $"{nameof(IDbConnectionFactory)} :: 9dc9ad38-b1f4-450a-ad39-e2c3a288afca";

    /// <summary>
    ///     Registers the data services.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    /// <param name="configure">
    ///     The configuration action.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection"/> so other actions
    ///     can be chained.
    /// </returns>
    public static IServiceCollection RegisterDatabaseServices(this IServiceCollection self, Action<SqliteOptions>? configure = null) {
        return self.Configure(configure ?? (_ => { }))
                   .InnerRegisterDataServices();
    }

    /// <summary>
    ///     Registers the data services.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    /// <param name="configuration">
    ///     The configuration.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection"/> so other actions
    ///     can be chained.
    /// </returns>
    public static IServiceCollection RegisterDatabaseServices(this IServiceCollection self, IConfiguration configuration) {
        var section = configuration.GetSection(nameof(SqliteOptions));

        return self.Configure<SqliteOptions>(section)
                   .InnerRegisterDataServices();
    }

    private static IServiceCollection InnerRegisterDataServices(this IServiceCollection self) {
        self.TryAddKeyedSingleton<IDbConnectionFactory, DbConnectionFactory>(DB_CONNECTION_FACTORY_KEY);
        self.TryAddTransient(ResolveDatabase);

        return self;
    }

    private static IDatabase ResolveDatabase(IServiceProvider provider) {
        var dbConnectionFactory = provider.GetRequiredKeyedService<IDbConnectionFactory>(DB_CONNECTION_FACTORY_KEY);
        var logger = provider.GetLogger<Database>();

        return new Database(dbConnectionFactory, logger);
    }
}