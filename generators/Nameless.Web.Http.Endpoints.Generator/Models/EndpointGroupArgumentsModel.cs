namespace Nameless.Web.Http.Endpoints.Generator.Models;

public record EndpointGroupArgumentsModel {
    public required string Name { get; init; }
    public required string Prefix { get; init; }
}