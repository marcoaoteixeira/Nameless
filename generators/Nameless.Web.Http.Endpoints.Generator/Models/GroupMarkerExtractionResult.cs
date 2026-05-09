using System.Collections.Immutable;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

internal sealed record GroupMarkerExtractionResult(
    GroupMarkerModel? Model,
    ImmutableArray<GeneratorDiagnostic> Diagnostics
) {
    internal static GroupMarkerExtractionResult Empty { get; } = new(Model: null, Diagnostics: []);

    internal static GroupMarkerExtractionResult Success(
        GroupMarkerModel model,
        ImmutableArray<GeneratorDiagnostic> diagnostics = default) {
        return new GroupMarkerExtractionResult(model, diagnostics.IsDefault ? [] : diagnostics);
    }

    internal static GroupMarkerExtractionResult Failure(ImmutableArray<GeneratorDiagnostic> diagnostics) {
        return new GroupMarkerExtractionResult(Model: null, diagnostics);
    }
}
