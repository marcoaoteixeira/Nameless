namespace Nameless.Web.Endpoints;
public sealed record OpenApiMetadata {
    public string[] Tags { get; init; } = [];
    public string Summary { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string GroupName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Produces[] Produces { get; init; } = [];
}
