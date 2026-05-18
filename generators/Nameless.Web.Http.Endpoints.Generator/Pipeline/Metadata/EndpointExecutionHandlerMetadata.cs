using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Nameless.Web.Http.Endpoints.Generator.Models;

namespace Nameless.Web.Http.Endpoints.Generator.Pipeline.Metadata;

internal record EndpointExecutionHandlerMetadata {
    internal IMethodSymbol? Handler { get; }
    internal ImmutableArray<HandlerParameterMetadata> Parameters { get; } = [];
    internal ImmutableArray<GeneratorDiagnostic> Diagnostics { get; } = [];

    private EndpointExecutionHandlerMetadata(IMethodSymbol? handler, HandlerParameterMetadata[] parameters, GeneratorDiagnostic[] diagnostics) {
        Handler = handler;
        Parameters = [.. parameters];
        Diagnostics = [.. diagnostics];
    }

    internal static EndpointExecutionHandlerMetadata Success(IMethodSymbol handler, HandlerParameterMetadata[] parameters) {
        return new EndpointExecutionHandlerMetadata(handler, parameters, diagnostics: []);
    }

    internal static EndpointExecutionHandlerMetadata Failure(params GeneratorDiagnostic[] diagnostics) {
        return new EndpointExecutionHandlerMetadata(handler: null, parameters: [], diagnostics);
    }
}