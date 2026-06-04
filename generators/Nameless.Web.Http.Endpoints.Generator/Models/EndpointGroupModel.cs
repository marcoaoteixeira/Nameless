using Nameless.Web.Http.Endpoints.Generator.Conventions;
using Nameless.Web.Http.Endpoints.Generator.Emitters;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

public record EndpointGroupModel : IEmitModel {
    public required ClassModel Class { get; init; }
    public required EndpointGroupArgumentsModel Arguments { get; init; }
    public required ConventionCollection Conventions { get; init; }
    public required EndpointModel[] Endpoints { get; init; }
    public required VersionModel[] ReportVersions { get; init; }
    public required LocationModel Location { get; init; }
}