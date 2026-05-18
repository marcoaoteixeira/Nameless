using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Nameless.Web.Http.Endpoints.Generator.Conventions;
using Nameless.Web.Http.Endpoints.Generator.Pipeline.Metadata;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

internal sealed record EndpointModel(
    string Namespace,
    string ClassName,

    string ClassAccessModifier,

    string HttpVerb,
    string Route,
    string? Group,

    IMethodSymbol? Handler,
    ImmutableArray<HandlerParameterMetadata> HandlerParameters,

    ImmutableArray<VersionModel> Versions,

    ImmutableArray<ConventionEntry> Conventions,

    string FilePath,
    int StartLine,
    int StartCharacter
) {
    internal string FullClassName => string.IsNullOrEmpty(Namespace) ? ClassName : $"{Namespace}.{ClassName}";
}
