using System.Collections.Immutable;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

internal sealed record GroupModel(
    string Name,
    string Prefix,
    ImmutableArray<VersionModel> Versions,
    ImmutableArray<EndpointModel> Endpoints
);
