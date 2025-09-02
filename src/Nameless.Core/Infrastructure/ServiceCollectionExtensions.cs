using Microsoft.Extensions.Configuration;
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
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="self"/> is <see langword="null"/>.
    /// </exception>
    public static IServiceCollection RegisterApplicationContext(this IServiceCollection self, Action<ApplicationContextOptions>? configure = null) {
        Guard.Against.Null(self);

        self.Configure(configure ?? (_ => { }));
        self.TryAddSingleton<IApplicationContext, ApplicationContext>();

        return self;
    }

    /// <summary>
    ///     Registers <see cref="IApplicationContext" /> implementation in the
    ///     service collection.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IServiceCollection" />.
    /// </param>
    /// <param name="configuration">
    ///     The configuration.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection" />, so other actions can
    ///     be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     if
    ///         <paramref name="self"/> or
    ///         <paramref name="configuration"/>
    ///     is <see langword="null"/>.
    /// </exception>
    public static IServiceCollection RegisterApplicationContext(this IServiceCollection self, IConfiguration configuration) {
        Guard.Against.Null(self);
        Guard.Against.Null(configuration);

        var section = configuration.GetSection(nameof(ApplicationContextOptions));

        self.Configure<ApplicationContextOptions>(section);
        self.TryAddSingleton<IApplicationContext, ApplicationContext>();

        return self;
    }
}