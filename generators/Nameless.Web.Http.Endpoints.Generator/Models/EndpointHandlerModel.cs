using Microsoft.CodeAnalysis;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

public record EndpointHandlerModel {
    public required IMethodSymbol Method { get; init; }
    public EndpointHandlerParameterModel[] Parameters { get; init; } = [];
}