namespace Nameless.Web.Http.Endpoints.Attributes.Versioning;

/// <summary>
///     Maps an endpoint to one or more API versions within its route group's
///     API version set.
/// </summary>
/// <remarks>
///     <para>
///         Multiple <see cref="VersionAttribute"/> instances may be applied
///         to the same endpoint class to make it available across several API
///         versions.
///     </para>
///     <para>
///         Endpoints without any <see cref="VersionAttribute"/> are mapped
///         to all versions declared on the group's API version set.
///     </para>
/// </remarks>
/// <param name="version">
///     The API version string, for example <c>"1.0"</c> or <c>"2.0"</c>.
///     Both <c>"1"</c> and <c>"1.0"</c> are treated as equivalent by
///     <c>Asp.Versioning.ApiVersion</c>.
/// </param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class VersionAttribute(string version) : Attribute {
    /// <summary>
    ///     Gets the API version string (e.g., <c>"1.0"</c>, <c>"2.0"</c>).
    ///     Parsed by <c>Asp.Versioning.ApiVersion</c>; both <c>"1"</c> and
    ///     <c>"1.0"</c> represent the same version.
    /// </summary>
    public string Version { get; } = version;

    /// <summary>
    ///     Gets or sets a value indicating whether this API version is
    ///     deprecated. Deprecated versions are still functional but are
    ///     flagged in the API version set metadata.
    /// </summary>
    public bool Deprecated { get; init; }
}
