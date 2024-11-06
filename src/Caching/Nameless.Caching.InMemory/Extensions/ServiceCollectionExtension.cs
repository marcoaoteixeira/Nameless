using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Caching.InMemory;

/// <summary>
/// Extension methods for In-Memory cache service.
/// </summary>
public static class ServiceCollectionExtension {
    /// <summary>
    /// Registers the current implementation of <see cref="ICache"/>.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/> instance.</param>
    /// <returns>The current <see cref="IServiceCollection"/> so other actions can be chained.</returns>
    public static IServiceCollection AddInMemoryCache(this IServiceCollection self)
        => Prevent.Argument
                  .Null(self)
                  .AddSingleton<ICache, InMemoryCache>();
}