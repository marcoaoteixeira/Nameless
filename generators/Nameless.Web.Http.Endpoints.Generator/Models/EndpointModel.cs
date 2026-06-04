using Nameless.Web.Http.Endpoints.Generator.Conventions;
using Nameless.Web.Http.Endpoints.Generator.Emitters;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

public record EndpointModel : IEmitModel {
    public required ClassModel Class { get; init; }
    public required EndpointArgumentsModel Arguments { get; init; }
    public required EndpointHandlerModel Handler { get; init; }
    public required ConventionCollection Conventions { get; init; }
    public required LocationModel Location { get; init; }
}