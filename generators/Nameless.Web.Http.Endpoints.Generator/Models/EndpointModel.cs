using Nameless.Web.Http.Endpoints.Generator.Conventions;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

public record EndpointModel {
    public required ClassModel Class { get; init; }
    public required EndpointArgumentsModel Arguments { get; init; }
    public required EndpointHandlerModel Handler { get; init; }
    public required ConventionCollection Conventions { get; init; }
    public required LocationModel Location { get; init; }
}