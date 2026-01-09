using Microsoft.AspNetCore.OpenApi;

namespace Nameless.Web.Endpoints;

/// <summary>
///     Represents the OpenAPI document options.
/// </summary>
public record OpenApiDocumentOptions {
    /// <summary>
    ///     Gets or init the OpenAPI document name.
    /// </summary>
    public required string DocumentName { get; init; }

    /// <summary>
    ///     Gets or init the OpenAPI options.
    /// </summary>
    public Action<OpenApiOptions>? Options { get; init; }
}