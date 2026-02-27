using Nameless.Web.MinimalEndpoints.Definitions.Metadata;

namespace Nameless.Web.MinimalEndpoints.Definitions;

/// <summary>
///     Defines the contract for building minimal endpoints.
/// </summary>
public interface IEndpointDescriptor {
    /// <summary>
    ///     Gets the type of the endpoint.
    /// </summary>
    Type EndpointType { get; }

    /// <summary>
    ///     Gets the HTTP method for the endpoint.
    /// </summary>
    string HttpMethod { get; }

    /// <summary>
    ///     Gets the route pattern for the endpoint.
    /// </summary>
    string RoutePattern { get; }

    /// <summary>
    ///     Gets the action to invoke on the endpoint.
    /// </summary>
    string ActionName { get; }

    /// <summary>
    ///     Gets the name of the endpoint.
    /// </summary>
    string? Name { get; }

    /// <summary>
    ///     Gets the group name for the endpoint.
    ///     This will act like a route prefix.
    /// </summary>
    string GroupName { get; }

    /// <summary>
    ///     Gets the display name of the endpoint.
    /// </summary>
    string? DisplayName { get; }

    /// <summary>
    ///     Gets the description of the endpoint.
    /// </summary>
    string? Description { get; }

    /// <summary>
    ///     Gets the summary of the endpoint.
    /// </summary>
    string? Summary { get; }

    /// <summary>
    ///     Gets the endpoint tags.
    /// </summary>
    string[] Tags { get; }

    /// <summary>
    ///     Gets the metadata for the version of the endpoint.
    /// </summary>
    VersionMetadata Version { get; }

    /// <summary>
    ///     Gets the request timeout policy for the endpoint.
    /// </summary>
    string? RequestTimeoutPolicy { get; }

    /// <summary>
    ///     Gets the name of the rate limiting policy to apply to the endpoint.
    /// </summary>
    string? RateLimitingPolicy { get; }

    /// <summary>
    ///     Gets the names of the authorization policies to apply to the endpoint.
    /// </summary>
    string[] AuthorizationPolicies { get; }

    /// <summary>
    ///     Gets the name of the CORS policy to apply to the endpoint.
    /// </summary>
    string? CorsPolicy { get; }

    /// <summary>
    ///     Gets the name of the output cache policy to apply to the endpoint.
    /// </summary>
    string? OutputCachePolicy { get; }

    /// <summary>
    ///     Whether the endpoint allows anonymous access.
    /// </summary>
    bool AllowAnonymous { get; }

    /// <summary>
    ///     Whether the endpoint should use anti-forgery protection.
    /// </summary>
    bool UseAntiforgery { get; }

    /// <summary>
    ///     Whether the HTTP metrics are disabled for the endpoint.
    /// </summary>
    bool DisableHttpMetrics { get; }

    /// <summary>
    ///     Gets the endpoint accept metadata.
    /// </summary>
    AcceptMetadata[] Accepts { get; }

    /// <summary>
    ///     Gets the endpoint produce metadata.
    /// </summary>
    ProduceMetadata[] Produces { get; }

    /// <summary>
    ///     Gets the filters to apply to the endpoint.
    /// </summary>
    Action<IEndpointFilterBuilder>[] Filters { get; }

    /// <summary>
    ///     Gets the additional metadata associated with the
    ///     endpoint.
    /// </summary>
    object[] AdditionalMetadata { get; }
}