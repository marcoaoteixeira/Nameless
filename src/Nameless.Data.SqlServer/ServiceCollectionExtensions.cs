using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Data.SqlServer;

/// <summary>
///     Extension methods for <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions {
    private const string DB_CONNECTION_FACTORY_KEY = $"{nameof(IDbConnectionFactory)} :: 23ea5d7a-e750-4aa1-a3c7-c95a0978c707";

    /// <summary>
    /// Registers the data services.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/>.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/> so other actions can be chained.
    /// </returns>
    public static IServiceCollection RegisterDatabase(this IServiceCollection self, Action<SqlServerOptions>? configure = null) {
        return self.Configure(configure ?? (_ => { }))
                   .AddKeyedSingleton<IDbConnectionFactory, DbConnectionFactory>(DB_CONNECTION_FACTORY_KEY)
                   .AddScoped(ResolveDatabase);
    }

    private static IDatabase ResolveDatabase(IServiceProvider provider) {
        var dbConnectionFactory = provider.GetRequiredKeyedService<IDbConnectionFactory>(DB_CONNECTION_FACTORY_KEY);
        var logger = provider.GetLogger<Database>();

        return new Database(dbConnectionFactory, logger);
    }
}