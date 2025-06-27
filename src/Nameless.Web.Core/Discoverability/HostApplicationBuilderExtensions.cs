using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nameless.Web.Discoverability;

/// <summary>
///     <see cref="IHostApplicationBuilder"/> extension methods.
/// </summary>
public static class HostApplicationBuilderExtensions {
    /// <summary>
    ///     Registers discovery services in the application builder.
    /// </summary>
    /// <typeparam name="THostApplicationBuilder">Type of the host application builder.</typeparam>
    /// <param name="self">The current <typeparamref name="THostApplicationBuilder"/>.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>
    ///     The current <typeparamref name="THostApplicationBuilder"/> instance so other actions can be chained.
    /// </returns>
    public static THostApplicationBuilder ConfigureServiceDiscovery<THostApplicationBuilder>(this THostApplicationBuilder self, Action<ServiceDiscoveryOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        var innerConfigure = configure ?? (_ => { });
        var options = new ServiceDiscoveryOptions();

        innerConfigure(options);

        self.Services
            .AddServiceDiscovery()
            .ConfigureHttpClientDefaults(http => {
                http.AddStandardResilienceHandler(options.ConfigureHttpStandardResilience); // Turn on resilience by default
                http.AddServiceDiscovery(); // Turn on service discovery by default
            });

        // Uncomment the following to restrict the allowed schemes for service discovery.
        // builder.Services.Configure<ServiceDiscoveryOptions>(options =>
        // {
        //     options.AllowedSchemes = ["https"];
        // });

        return self;
    }
}