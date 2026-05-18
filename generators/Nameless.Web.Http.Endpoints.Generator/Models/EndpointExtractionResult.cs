using System.Collections.Immutable;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

internal sealed record EndpointExtractionResult {
    internal EndpointModel? Model { get; }
    internal ImmutableArray<GeneratorDiagnostic> Diagnostics { get; }

    private EndpointExtractionResult(EndpointModel? model, ImmutableArray<GeneratorDiagnostic> diagnostics) {
        Model = model;
        Diagnostics = diagnostics;
    }

    internal static EndpointExtractionResult Success(EndpointModel model) {
        return new EndpointExtractionResult(model, diagnostics: []);
    }

    internal static EndpointExtractionResult Failure(params GeneratorDiagnostic[] diagnostics) {
        return new EndpointExtractionResult(model: null, [.. diagnostics]);
    }

    internal static EndpointExtractionResult Failure(ImmutableArray<GeneratorDiagnostic> diagnostics) {
        return new EndpointExtractionResult(model: null, diagnostics);
    }
}
