using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

namespace Nameless.Web.Endpoints;

/// <summary>
///     Represents options for configuring endpoints.
/// </summary>
public sealed class EndpointOptions {
    /// <summary>
    ///     Gets or sets the assemblies to scan for endpoints.
    /// </summary>
    public Assembly[] Assemblies { get; set; } = [];

    /// <summary>
    ///     Gets or sets the action to configure OpenAPI options.
    /// </summary>
    public Func<OpenApiDocumentOptions[]>? ConfigureOpenApi { get; set; }

    /// <summary>
    ///     Gets or sets the action to configure API versioning options.
    /// </summary>
    public Action<ApiVersioningOptions>? ConfigureApiVersioning { get; set; }

    /// <summary>
    ///     Gets or sets the action to configure API explorer options.
    /// </summary>
    public Action<ApiExplorerOptions>? ConfigureApiExplorer { get; set; }
}