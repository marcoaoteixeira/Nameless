using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.Infrastructure;

public static class ServiceCollectionExtensions {
    /// <summary>
    ///     Registers <see cref="IApplicationContext" /> implementation in the
    ///     service collection.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IServiceCollection" />.
    /// </param>
    /// <param name="configure">
    ///     The configuration action.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection" />, so other actions can
    ///     be chained.
    /// </returns>
    public static IServiceCollection RegisterApplicationContext(this IServiceCollection self, Action<ApplicationContextOptions>? configure = null) {
        self.Configure(configure ?? (_ => { }));

        self.TryAddSingleton<IApplicationContext, ApplicationContext>();

        return self;
    }
}