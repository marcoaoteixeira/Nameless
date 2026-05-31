using Nameless.Web.Http.Endpoints.Generator.Conventions;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

public record GroupingModel {
    public required ClassModel Class { get; init; }
    public required EndpointGroupArgumentsModel Arguments { get; init; }
    public required VersionModel[] ReportVersions { get; init; }
    public required EndpointModel[] Endpoints { get; init; }
    public required ConventionCollection Conventions { get; init; }
}
