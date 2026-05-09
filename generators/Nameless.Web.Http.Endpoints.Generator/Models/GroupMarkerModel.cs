using System.Collections.Immutable;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

internal sealed record GroupMarkerModel(
    string Name,
    string Prefix,
    string TypeFqn,
    ImmutableArray<VersionModel> DeclaredVersions
);
