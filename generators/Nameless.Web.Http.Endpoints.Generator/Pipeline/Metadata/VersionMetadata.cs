using System.Collections.Immutable;
using Nameless.Web.Http.Endpoints.Generator.Models;

namespace Nameless.Web.Http.Endpoints.Generator.Pipeline.Metadata;

internal record VersionMetadata(
    ImmutableArray<VersionModel> Versions,
    ImmutableArray<GeneratorDiagnostic> Diagnostics
) {
    internal static VersionMetadata Empty => new(Versions: [], Diagnostics: []);
}