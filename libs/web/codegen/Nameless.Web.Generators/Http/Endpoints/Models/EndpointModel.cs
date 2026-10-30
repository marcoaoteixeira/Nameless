using Nameless.Generators.Emitters;
using Nameless.Generators.Models;
using Nameless.Web.Generators.Http.Endpoints.Conventions;

namespace Nameless.Web.Generators.Http.Endpoints.Models;

public record EndpointModel : IClassEmitModel {
    public required ClassModel Class { get; init; }
    public required EndpointArgumentsModel Arguments { get; init; }
    public required EndpointHandlerModel Handler { get; init; }
    public required ConventionCollection Conventions { get; init; }
    public required LocationModel Location { get; init; }
}