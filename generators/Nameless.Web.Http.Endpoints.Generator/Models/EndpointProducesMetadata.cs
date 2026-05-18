namespace Nameless.Web.Http.Endpoints.Generator.Models;

internal sealed record EndpointProducesMetadata(
    string FullTypeName,
    int StatusCode,
    string? ContentType,
    ProducesKind Kind
);
