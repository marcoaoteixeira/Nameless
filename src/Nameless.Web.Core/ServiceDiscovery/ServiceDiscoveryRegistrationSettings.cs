using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.ServiceDiscovery;

namespace Nameless.Web.ServiceDiscovery;

/// <summary>
///     Services discovery registration settings.
/// </summary>
public class ServiceDiscoveryRegistrationSettings {
    /// <summary>
    ///     Gets or sets the action to configure service discovery options.
    /// </summary>
    public Action<ServiceDiscoveryOptions> ConfigureServiceDiscovery { get; set; } = _ => { };

    /// <summary>
    ///     Gets or sets the action to configure HTTP standard resilience options.
    /// </summary>
    public Action<HttpStandardResilienceOptions> ConfigureStandardResilienceHandler { get; set; } = _ => { };
}