namespace Nameless.Web.Endpoints;

/// <summary>
/// Represents endpoint information about versioning.
/// </summary>
public sealed record Versioning {
    /// <summary>
    /// Gets or init the version.
    /// </summary>
    public int Version { get; init; }

    /// <summary>
    /// Gets or init whether the endpoint is deprecated.
    /// </summary>
    public bool IsDeprecated { get; init; }

    /// <summary>
    /// Gets or init the version to redirect.
    /// </summary>
    public int MapToVersion { get; init; }

    /// <summary>
    /// Creates versioning information.
    /// </summary>
    /// <param name="version">The endpoint version.</param>
    /// <param name="isDeprecated">Whether it is deprecated or not.</param>
    /// <param name="mapToVersion">The version to redirect.</param>
    /// <returns></returns>
    public static Versioning For(int version, bool isDeprecated = false, int mapToVersion = 0)
        => new() {
            Version = Prevent.Argument.LowerThan(version, minValue: 0),
            IsDeprecated = isDeprecated,
            MapToVersion = Prevent.Argument.LowerThan(mapToVersion, minValue: 0)
        };
}