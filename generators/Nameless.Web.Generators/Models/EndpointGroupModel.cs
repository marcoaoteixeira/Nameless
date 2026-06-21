using Nameless.Web.Generators.Conventions;
using Nameless.Web.Generators.Emitters;

namespace Nameless.Web.Generators.Models;

public record EndpointGroupModel : IEmitModel {
    public required ClassModel Class { get; init; }
    public required EndpointGroupArgumentsModel Arguments { get; init; }
    public required ConventionCollection Conventions { get; init; }
    public required EndpointModel[] Endpoints { get; init; }
    public required VersionModel[] ReportVersions { get; init; }
    public required LocationModel Location { get; init; }
}