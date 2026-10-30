using Nameless.Generators.Emitters;
using Nameless.Generators.Models;
using Nameless.Web.Generators.Http.Endpoints.Conventions;

namespace Nameless.Web.Generators.Http.Endpoints.Models;

public record EndpointGroupModel : IClassEmitModel {
    public required ClassModel Class { get; init; }
    public required EndpointGroupArgumentsModel Arguments { get; init; }
    public required ConventionCollection Conventions { get; init; }
    public required EndpointModel[] Endpoints { get; init; }
    public required VersionModel[] ReportVersions { get; init; }
    public required LocationModel Location { get; init; }
}