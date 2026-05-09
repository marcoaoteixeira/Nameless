using System.Collections.Immutable;
using Nameless.Web.Http.Endpoints.Generator.Models;

namespace Nameless.Web.Http.Endpoints.Generator.Pipeline;

internal readonly record struct GroupCollectionMetadata(
    ImmutableArray<GroupModel> Groups,
    ImmutableArray<GeneratorDiagnostic> Diagnostics
);