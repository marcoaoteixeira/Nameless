namespace Nameless.Web.Http.Endpoints.Generator.Models;

internal sealed record ParameterModel(
    string Name,
    string FullTypeName,
    bool IsCancellationToken,
    ParameterBindingKind BindingKind,
    string? BindingName
);
