using System.Collections.Immutable;

namespace Nameless.Web.Http.Endpoints.Generator.Pipeline;

internal readonly record struct EndpointMetadata(
    string HttpMethod,
    string RouteTemplate,
    string? EndpointName,
    ImmutableArray<string> Tags
);