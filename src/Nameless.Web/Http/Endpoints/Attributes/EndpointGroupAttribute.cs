using System.Diagnostics.CodeAnalysis;

namespace Nameless.Web.Http.Endpoints.Attributes;

/// <summary>
///     Marks a class as the definition for a named route group.
/// </summary>
/// <remarks>
///     Apply this attribute to a <c>partial</c> group marker class
///     - not to endpoint classes. Every endpoint that belongs to this group
///     references the marker class via
///     <see cref="EndpointAttribute{THttpVerb}.Group"/>.
///     The source generator reads <see cref="Prefix"/> from this marker class
///     and emits a single <c>app.MapGroup(prefix)</c> call shared by all
///     member endpoints. The <see cref="Prefix"/> supports ASP.NET Core route
///     constraint syntax, including the API versioning constraint
///     (e.g., <c>"/api/v{version:apiVersion}/users"</c>).
/// </remarks>
/// <param name="name">
///     The logical name of the route group.
/// </param>
/// <param name="prefix">
///     The route prefix applied to every endpoint in the group.
/// </param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class EndpointGroupAttribute(string name, [StringSyntax("Route")] string prefix) : Attribute {
    /// <summary>
    ///     Gets the logical name of the route group.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    ///     Gets the route prefix applied to every endpoint in this group
    ///     (e.g., <c>"/api/users"</c> or
    ///     <c>"/api/v{version:apiVersion}/users"</c>).
    /// </summary>
    public string Prefix { get; } = prefix;

    /// <summary>
    ///     Gets the OpenAPI description for the group.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     Gets the OpenAPI summary associated with the group.
    /// </summary>
    public string? Summary { get; init; }
}
