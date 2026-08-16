using Nameless.Generators.Emitters;
using Nameless.Generators.Models;
using Nameless.Generators.Web.Http.Endpoints.Conventions;

namespace Nameless.Generators.Web.Http.Endpoints.Models;

public record EndpointGroupModel : IEmitModel {
    public required ClassModel Class { get; init; }
    public required EndpointGroupArgumentsModel Arguments { get; init; }
    public required ConventionCollection Conventions { get; init; }
    public required EndpointModel[] Endpoints { get; init; }
    public required VersionModel[] ReportVersions { get; init; }
    public required LocationModel Location { get; init; }
}