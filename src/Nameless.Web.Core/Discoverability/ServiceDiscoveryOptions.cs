using Microsoft.Extensions.Http.Resilience;

namespace Nameless.Web.Discoverability;

/// <summary>
///     Discovery services options for configuring service discovery in the application.
/// </summary>
public sealed record ServiceDiscoveryOptions {
    /// <summary>
    ///     Gets or sets the action to configure HTTP standard resilience options.
    /// </summary>
    public Action<HttpStandardResilienceOptions> ConfigureHttpStandardResilience { get; set; } = _ => { };
}