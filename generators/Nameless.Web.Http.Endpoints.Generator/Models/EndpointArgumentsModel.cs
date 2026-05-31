using Nameless.Web.Http.Endpoints.Generator.Infrastructure;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

public record EndpointArgumentsModel {
    public HttpVerbs HttpVerb { get; init; }
    public string Route { get; init; } = string.Empty;
    public string Group { get; init; } = string.Empty;
    public VersionModel Version { get; init; } = VersionModel.V1;
}