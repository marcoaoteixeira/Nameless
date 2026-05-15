using System.Collections.Immutable;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

internal sealed record GroupModel(
    string Name,
    string Prefix,
    string? ClassName,
    string Namespace,
    string AccessModifier,
    ImmutableArray<VersionModel> Versions,
    ImmutableArray<EndpointModel> Endpoints,
    string? RateLimitingPolicy,
    bool DisableRateLimiting,
    bool? RequireAntiforgery,
    bool DisableHttpMetrics,
    string? OutputCachePolicy,
    string? CorsPolicy,
    bool AllowAnonymous,
    bool RequireAuthorization,
    string? AuthorizationPolicy,
    string? RequestTimeoutPolicy,
    bool DisableRequestTimeout,
    bool AllowCookieRedirect,
    ImmutableArray<string> FilterTypeNames
) {
    // True when the group corresponds to a user-defined partial class (has a ClassName).
    // False for synthetic groups created for ungrouped versioned endpoints.
    internal bool IsExplicit => ClassName is not null;

    internal string FullClassName => string.IsNullOrEmpty(Namespace)
        ? ClassName!
        : $"{Namespace}.{ClassName}";
}
