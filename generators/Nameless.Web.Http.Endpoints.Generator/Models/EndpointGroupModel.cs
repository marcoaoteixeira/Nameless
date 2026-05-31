using Nameless.Web.Http.Endpoints.Generator.Conventions;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

public record EndpointGroupModel {
    public required ClassModel Class { get; init; }
    public required EndpointGroupArgumentsModel Arguments { get; init; }
    public required ConventionCollection Conventions { get; init; }
    public required LocationModel Location { get; init; }
}