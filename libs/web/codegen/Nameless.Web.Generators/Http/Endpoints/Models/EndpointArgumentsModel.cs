using Nameless.Web.Generators.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Generators.Http.Endpoints.Models;

public record EndpointArgumentsModel {
    public HttpVerbs HttpVerb { get; init; }
    public string Route { get; init; } = string.Empty;
    public string Group { get; init; } = string.Empty;
    public VersionModel Version { get; init; } = VersionModel.V1;
}