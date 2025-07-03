using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Infrastructure;

public static class ServiceCollectionExtensions {
    /// <summary>
    ///     Registers <see cref="IApplicationContext" /> implementation in the service collection.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection" />.</param>
    /// <returns>
    ///     The current <see cref="IServiceCollection" />, so other actions can be chained.
    /// </returns>
    public static IServiceCollection RegisterApplicationContext(this IServiceCollection self, Action<ApplicationContextOptions>? configure = null) {
        return self
               .Configure(configure ?? (_ => { }))
               .AddSingleton<IApplicationContext, ApplicationContext>();
    }
}