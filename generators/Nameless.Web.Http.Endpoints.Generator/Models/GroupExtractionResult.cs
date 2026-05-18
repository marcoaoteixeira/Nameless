using System.Collections.Immutable;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

internal sealed record GroupExtractionResult(
    GroupMarkerModel? Model,
    ImmutableArray<GeneratorDiagnostic> Diagnostics
) {
    internal static GroupExtractionResult Empty { get; } = new(Model: null, Diagnostics: []);

    internal static GroupExtractionResult Success(
        GroupMarkerModel model,
        ImmutableArray<GeneratorDiagnostic> diagnostics = default) {
        return new GroupExtractionResult(model, diagnostics.IsDefault ? [] : diagnostics);
    }

    internal static GroupExtractionResult Failure(params GeneratorDiagnostic[] diagnostics) {
        return new GroupExtractionResult(Model: null, Diagnostics: [.. diagnostics]);
    }
}
