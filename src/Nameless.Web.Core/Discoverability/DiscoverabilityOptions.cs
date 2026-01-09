using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.ServiceDiscovery;

namespace Nameless.Web.Discoverability;

/// <summary>
///     Discovery services options for configuring service discovery in the application.
/// </summary>
public class DiscoverabilityOptions {
    /// <summary>
    ///     Gets or sets the action to configure service discovery options.
    /// </summary>
    public Action<ServiceDiscoveryOptions>? ConfigureServiceDiscovery { get; set; }

    /// <summary>
    ///     Gets or sets the action to configure HTTP standard resilience options.
    /// </summary>
    public Action<HttpStandardResilienceOptions>? ConfigureHttpStandardResilience { get; set; }
}