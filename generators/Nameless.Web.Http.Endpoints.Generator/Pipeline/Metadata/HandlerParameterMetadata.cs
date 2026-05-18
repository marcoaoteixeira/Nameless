using Nameless.Web.Http.Endpoints.Generator.Models;

namespace Nameless.Web.Http.Endpoints.Generator.Pipeline.Metadata;

internal sealed record HandlerParameterMetadata(
    string Name,
    string FullTypeName,
    bool IsCancellationToken,
    ParameterBindingKind BindingKind,
    string? BindingName
);