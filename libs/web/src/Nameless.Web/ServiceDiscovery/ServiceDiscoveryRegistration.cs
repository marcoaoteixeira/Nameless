using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.ServiceDiscovery;

namespace Nameless.Web.ServiceDiscovery;

/// <summary>
///     Services discovery registration settings.
/// </summary>
public class ServiceDiscoveryRegistration {
    /// <summary>
    ///     Whether it should override the service discovery configuration.
    /// </summary>
    public bool OverrideServiceDiscoveryConfiguration { get; set; }

    /// <summary>
    ///     Gets or sets the action to configure service discovery options.
    /// </summary>
    public Action<ServiceDiscoveryOptions>? ConfigureServiceDiscovery { get; set; }

    /// <summary>
    ///     Whether it should override the HTTP standard resilience configuration.
    /// </summary>
    public bool OverrideHttpStandardResilience { get; set; }

    /// <summary>
    ///     Gets or sets the action to configure HTTP standard resilience options.
    /// </summary>
    public Action<HttpStandardResilienceOptions>? ConfigureHttpStandardResilience { get; set; }
}