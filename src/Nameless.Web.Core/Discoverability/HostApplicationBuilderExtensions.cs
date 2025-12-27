using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nameless.Web.Discoverability;

/// <summary>
///     Host application builder extension methods.
/// </summary>
public static class HostApplicationBuilderExtensions {
    /// <summary>
    ///     Registers discovery services in the application builder.
    /// </summary>
    /// <typeparam name="THostApplicationBuilder">
    ///     Type that implements <see cref="IHostApplicationBuilder"/>.
    /// </typeparam>
    /// <param name="self">
    ///     The current <typeparamref name="THostApplicationBuilder"/>.
    /// </param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>
    ///     The current <typeparamref name="THostApplicationBuilder"/>
    ///     instance so other actions can be chained.
    /// </returns>
    public static THostApplicationBuilder RegisterDiscoverability<THostApplicationBuilder>(
        this THostApplicationBuilder self, Action<DiscoverabilityOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        var innerConfigure = configure ?? (_ => { });
        var options = new DiscoverabilityOptions();

        innerConfigure(options);

        self.Services
            .AddServiceDiscovery(options.ConfigureServiceDiscovery ?? (_ => { }))
            .ConfigureHttpClientDefaults(http => {
                // Turn on resilience by default
                http.AddStandardResilienceHandler(options.ConfigureHttpStandardResilience ?? (_ => { }));

                // Turn on service discovery by default
                http.AddServiceDiscovery();
            });

        return self;
    }
}