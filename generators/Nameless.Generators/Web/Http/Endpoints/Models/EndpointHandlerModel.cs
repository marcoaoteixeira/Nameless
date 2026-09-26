using Microsoft.CodeAnalysis;

namespace Nameless.Generators.Web.Http.Endpoints.Models;

public record EndpointHandlerModel {
    public required IMethodSymbol Method { get; init; }
    public EndpointHandlerParameterModel[] Parameters { get; init; } = [];
}