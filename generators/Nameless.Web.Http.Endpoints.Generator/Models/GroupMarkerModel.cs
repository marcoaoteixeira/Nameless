using System.Collections.Immutable;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

internal sealed record GroupMarkerModel(
    string Name,
    string Prefix,
    string TypeFqn,
    string ClassName,
    string Namespace,
    string AccessModifier,
    ImmutableArray<VersionModel> DeclaredVersions,
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
);
