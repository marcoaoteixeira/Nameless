using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;
using Nameless.Web.ServiceDiscovery;

namespace Nameless.Web.Hosting.Features;

/// <summary>
///     Registers service discovery (Aspire).
/// </summary>
public class ServiceDiscoveryWebHostFeature : HostFeature<ServiceDiscoveryRegistration> {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature);

        builder.Services
            .AddServiceDiscovery(serviceDiscovery => {
                if (!options.OverrideServiceDiscoveryConfiguration) {
                    /* default service discovery configuration */
                }

                options.ConfigureServiceDiscovery?.Invoke(serviceDiscovery);
            })
            .ConfigureHttpClientDefaults(httpClient => {
                // Turn on service discovery by default, no opt-out.
                httpClient.AddServiceDiscovery();

                // Turn on resilience by default
                _ = options.OverrideHttpStandardResilience
                    ? httpClient.AddStandardResilienceHandler(
                        resilience => options.ConfigureHttpStandardResilience?.Invoke(resilience)
                    )
                    : httpClient.AddStandardResilienceHandler();
            });
    }
}
