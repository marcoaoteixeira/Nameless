namespace Nameless.Web.Http.Endpoints.Attributes;

/// <summary>
///     Marks a class as the definition for a named route group.
/// </summary>
/// <remarks>
///     Apply this attribute to an empty marker class — not to endpoint
///     classes. Every endpoint that belongs to this group references the
///     marker class via <see cref="EndpointAttribute{THttpVerb}.Group"/>.
///     The source generator reads <see cref="Name"/>, <see cref="Prefix"/>,
///     and <see cref="Versions"/> from this marker class and emits a single
///     <c>app.MapGroup(prefix)</c> call shared by all member endpoints. The
///     <see cref="Prefix"/> supports ASP.NET Core route constraint syntax,
///     including the API versioning constraint
///     (e.g., <c>"/api/v{version:apiVersion}/users"</c>).
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class GroupAttribute : Attribute {
    /// <summary>
    ///     Gets the logical name of the route group.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the route prefix applied to every endpoint in this group
    ///     (e.g., <c>"/api/users"</c> or
    ///     <c>"/api/v{version:apiVersion}/users"</c>).
    /// </summary>
    public string Prefix { get; }

    /// <summary>
    ///     Gets or sets the API version strings supported by this group
    ///     (e.g., <c>["1.0", "2.0"]</c>).
    /// </summary>
    /// <remarks>
    ///     When set, these values define the API version set registered
    ///     for the group's <c>MapGroup</c> call. Individual endpoints
    ///     within the group still use <see cref="VersionAttribute"/> to
    ///     declare which version(s) they respond to. When omitted, the
    ///     version set is inferred by union-collecting all
    ///     <see cref="VersionAttribute"/> declarations from the group's
    ///     member endpoints.
    /// </remarks>
    public string[]? Versions { get; init; }

    /// <summary>
    ///     Initializes a new instance of <see cref="GroupAttribute"/> with
    ///     the specified name and prefix.
    /// </summary>
    /// <param name="name">
    ///     The logical group name shared by all endpoints in this group.
    /// </param>
    /// <param name="prefix">
    ///     The route prefix applied to every endpoint in the group.
    /// </param>
    public GroupAttribute(string name, string prefix) {
        Name = name;
        Prefix = prefix;
    }
}
