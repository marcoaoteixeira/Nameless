using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.EntityFrameworkCore;

/// <summary>
///     Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    ///     Registers the <see cref="DbContext"/> wrapper.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection"/> so other actions can
    ///     be chained.
    /// </returns>
    /// <remarks>
    ///     Recommendation is to use the <see cref="DbContextWrapper"/> when
    ///     you do not have access to add the interface <see cref="IDbContext"/> to
    ///     the actual <see cref="DbContext"/> implementation.
    /// </remarks>
    public static IServiceCollection RegisterDbContextWrapper(this IServiceCollection self) {
        self.TryAddTransient<IDbContext, DbContextWrapper>();

        return self;
    }
}
