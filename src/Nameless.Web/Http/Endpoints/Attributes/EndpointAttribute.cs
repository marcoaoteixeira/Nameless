using System.Diagnostics.CodeAnalysis;

namespace Nameless.Web.Http.Endpoints.Attributes;

/// <summary>
///     Declares a class as a Minimal API endpoint that handles HTTP requests
///     using the specified verb.
/// </summary>
/// <typeparam name="THttpVerb">
///     The HTTP verb type (e.g., <see cref="Get"/>, <see cref="Post"/>,
///     <see cref="Put"/>, <see cref="Delete"/>). Must implement
///     <see cref="IHttpVerb"/>.
/// </typeparam>
/// <remarks>
///     The source generator reads this attribute to emit the corresponding
///     <c>Map[Verb](route, handler)</c> call inside the generated
///     <c>MapEndpoints()</c> extension method. Use <see cref="Group"/> to
///     place the endpoint inside a route group defined by a
///     <see cref="GroupAttribute"/> marker class.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class EndpointAttribute<THttpVerb> : Attribute
    where THttpVerb : IHttpVerb {
    /// <summary>
    ///     Gets the route pattern for this endpoint, relative to the group
    ///     prefix when <see cref="Group"/> is set.
    /// </summary>
    public string Route { get; }

    /// <summary>
    ///     Gets or sets an optional name for the endpoint, used for
    ///     route-based link generation via <c>LinkGenerator</c>.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    ///     Gets or sets optional OpenAPI tags that categorise the endpoint
    ///     in the generated API document.
    /// </summary>
    public string[]? Tags { get; init; }

    /// <summary>
    ///     Gets or sets the marker class that defines the route group this
    ///     endpoint belongs to.
    /// </summary>
    /// <remarks>
    ///     The referenced type must be a class decorated with
    ///     <see cref="GroupAttribute"/>. Use <c>typeof(MyGroup)</c> to assign
    ///     membership (e.g., <c>Group = typeof(DemoGroup)</c>). Omit this
    ///     property to leave the endpoint ungrouped.
    /// </remarks>
    public Type? Group { get; init; }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="EndpointAttribute{THttpVerb}"/> with the specified
    ///     route pattern.
    /// </summary>
    /// <param name="route">
    ///     The route pattern for this endpoint. May be empty when the
    ///     endpoint sits at the group root. Supports all ASP.NET Core route
    ///     constraint syntax (e.g., <c>"/{id:int}"</c>).
    /// </param>
    public EndpointAttribute([StringSyntax("Route")] string? route = null) {
        Route = route ?? string.Empty;
    }
}
