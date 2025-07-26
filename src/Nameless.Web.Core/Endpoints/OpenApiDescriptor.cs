using Microsoft.AspNetCore.OpenApi;

namespace Nameless.Web.Endpoints;

/// <summary>
///     Represents a descriptor for an OpenAPI document.
/// </summary>
public record OpenApiDescriptor {
    /// <summary>
    ///     Gets or init the OpenAPI document name.
    /// </summary>
    public required string DocumentName { get; init; }

    /// <summary>
    ///     Gets or init the OpenAPI options.
    /// </summary>
    public Action<OpenApiOptions>? Options { get; init; }
}