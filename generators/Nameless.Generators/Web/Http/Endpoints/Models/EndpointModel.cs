using Nameless.Generators.Shared.Emitters;
using Nameless.Generators.Shared.Models;
using Nameless.Generators.Web.Http.Endpoints.Conventions;

namespace Nameless.Generators.Web.Http.Endpoints.Models;

public record EndpointModel : IEmitModel {
    public required ClassModel Class { get; init; }
    public required EndpointArgumentsModel Arguments { get; init; }
    public required EndpointHandlerModel Handler { get; init; }
    public required ConventionCollection Conventions { get; init; }
    public required LocationModel Location { get; init; }
}