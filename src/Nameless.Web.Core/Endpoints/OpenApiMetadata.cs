namespace Nameless.Web.Endpoints;

/// <summary>
/// Represents the endpoint information for Open API explorer.
/// </summary>
public sealed record OpenApiMetadata {
    /// <summary>
    /// Gets or init the description.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets or init the group name.
    /// </summary>
    public string GroupName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or init the name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets or init the list of results produced by the endpoint.
    /// </summary>
    public Produces[] Produces { get; init; } = [];

    /// <summary>
    /// Gets or init the summary.
    /// </summary>
    public string Summary { get; init; } = string.Empty;

    /// <summary>
    /// Gets or init the tags.
    /// </summary>
    public string[] Tags { get; init; } = [];
}
