namespace Nameless.Web.Http.Endpoints.Generator.Models;

internal sealed record ProducesModel(
    string FullTypeName,
    int StatusCode,
    string? ContentType,
    ProducesKind Kind
);
