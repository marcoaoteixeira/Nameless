using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Web.Discoverability;

/// <summary>
///     <see cref="WebApplicationBuilder"/> extension methods.
/// </summary>
public static class WebApplicationBuilderExtensions {
    /// <summary>
    ///     Registers discovery services in the application builder.
    /// </summary>
    /// <param name="self">The current <see cref="WebApplicationBuilder"/>.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>
    ///     The current <see cref="WebApplicationBuilder"/> instance so other actions can be chained.
    /// </returns>
    /// <remarks>
    ///     This will add the ser
    /// </remarks>
    public static WebApplicationBuilder RegisterDiscoverability(this WebApplicationBuilder self, Action<DiscoverabilityOptions>? configure = null) {
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