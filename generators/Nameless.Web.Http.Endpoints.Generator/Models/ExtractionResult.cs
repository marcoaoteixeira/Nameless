using System.Collections.Immutable;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

internal sealed record ExtractionResult(
    EndpointModel? Model,
    ImmutableArray<GeneratorDiagnostic> Diagnostics
) {
    internal static ExtractionResult Success(
        EndpointModel model,
        ImmutableArray<GeneratorDiagnostic> diagnostics = default) {
        return new ExtractionResult(model, diagnostics.IsDefault ? [] : diagnostics);
    }

    internal static ExtractionResult Failure(ImmutableArray<GeneratorDiagnostic> diagnostics) {
        return new ExtractionResult(null, diagnostics);
    }
}
