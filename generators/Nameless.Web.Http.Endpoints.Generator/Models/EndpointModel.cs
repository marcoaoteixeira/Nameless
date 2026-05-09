using System.Collections.Immutable;
using Nameless.Web.Http.Endpoints.Generator.Pipeline;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

internal sealed record EndpointModel(
    string ClassName,
    string Namespace,
    EndpointMetadata Metadata,
    string? GroupTypeFqn,
    ImmutableArray<VersionModel> Versions,
    ImmutableArray<ParameterModel> Parameters,
    ImmutableArray<ProducesModel> Produces,
    ImmutableArray<string> FilterTypeNames,
    string? AcceptsTypeName,
    string? AcceptsContentType,
    bool UseAntiforgery,
    string? Summary,
    string? Description,
    bool RequiresAuthorization,
    string? AuthorizationPolicy,
    bool AllowAnonymous,
    string? CorsPolicy,
    string? RateLimitingPolicy,
    string? OutputCachePolicy,
    string? RequestTimeoutPolicy,
    bool DisableHttpMetrics,

    string FilePath,
    int StartLine,
    int StartCharacter
) {
    internal string FullClassName => string.IsNullOrEmpty(Namespace)
        ? ClassName
        : $"{Namespace}.{ClassName}";
}
