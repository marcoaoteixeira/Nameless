using Microsoft.AspNetCore.Authorization;

namespace Nameless.Web.Endpoints;

/// <summary>
/// Base class to implement minimal API endpoints.
/// </summary>
public abstract class EndpointBase {
    /// <summary>
    /// Gets the HTTP method for the endpoint. Default value is "GET".
    /// </summary>
    public virtual string HttpMethod => Root.HttpMethods.GET;

    /// <summary>
    /// Gets the endpoint route pattern.
    /// </summary>
    public abstract string RoutePattern { get; }

    /// <summary>
    /// Whether it should use validation service filter. Default value is false.
    /// </summary>
    public virtual bool UseValidationFilter => false;

    /// <summary>
    /// Creates the endpoint request handler.
    /// </summary>
    /// <returns>
    /// A <see cref="Delegate"/> that represents the endpoint request handler.
    /// </returns>
    public abstract Delegate CreateDelegate();

    /// <summary>
    /// Retrieves the accepts HTTP headers for this endpoint.
    /// </summary>
    /// <returns>
    /// A list of accepts HTTP headers.
    /// </returns>
    public virtual IEnumerable<Accepts> GetAccepts()
        => [];

    /// <summary>
    /// Retrieves a list of authorization directives for this endpoint.
    /// </summary>
    /// <returns>
    /// A list of authorization directives.
    /// </returns>
    public virtual IEnumerable<IAuthorizeData> GetAuthorize()
        => [];

    /// <summary>
    /// Retrieves the versioning information for the endpoint.
    /// </summary>
    /// <returns>
    /// The versioning information for the endpoint.
    /// </returns>
    public virtual Versioning GetVersioningInfo()
        => Versioning.For(version: 1);

    /// <summary>
    /// Retrieves endpoint information for Open API.
    /// </summary>
    /// <returns>
    /// Endpoint information for Open API.
    /// </returns>
    public virtual OpenApiMetadata GetOpenApiMetadata()
        => new() {
            Description = string.Empty,
            GroupName = string.Empty,
            Name = GetType().Name,
            Produces = [],
            Summary = string.Empty,
            Tags = []
        };
}