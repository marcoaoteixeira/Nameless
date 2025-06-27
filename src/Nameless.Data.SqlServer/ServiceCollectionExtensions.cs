using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Data.SqlServer;

/// <summary>
///     Extension methods for <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// Registers the data services.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/>.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/> so other actions can be chained.
    /// </returns>
    public static IServiceCollection ConfigureDataServices(this IServiceCollection self, Action<SqlServerOptions>? configure = null) {
        return self.Configure(configure ?? (_ => { }))
                   .RegisterMainServices();
    }

    private static IServiceCollection RegisterMainServices(this IServiceCollection self) {
        return self.AddSingleton<IDbConnectionFactory, DbConnectionFactory>()
                   .AddSingleton<IDatabase, Database>();
    }
}