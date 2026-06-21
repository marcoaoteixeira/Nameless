using Nameless.Web.Generators.Conventions;
using Nameless.Web.Generators.Emitters;

namespace Nameless.Web.Generators.Models;

public record EndpointModel : IEmitModel {
    public required ClassModel Class { get; init; }
    public required EndpointArgumentsModel Arguments { get; init; }
    public required EndpointHandlerModel Handler { get; init; }
    public required ConventionCollection Conventions { get; init; }
    public required LocationModel Location { get; init; }
}