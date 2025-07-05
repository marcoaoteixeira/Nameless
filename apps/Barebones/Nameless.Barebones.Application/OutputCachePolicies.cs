namespace Nameless.Barebones.Application;

/// <summary>
///     Struct representing an output cache policy with a name and expiration time.
/// </summary>
/// <param name="PolicyName">The policy name.</param>
/// <param name="Expiration">The expiration time.</param>
public readonly record struct OutputCachePolicy(string PolicyName, TimeSpan Expiration) {
    /// <summary>
    ///     Gets a predefined output cache policy for OpenAPI documentation.
    /// </summary>
    public static readonly OutputCachePolicy OpenApiDocumentation = new(nameof(OpenApiDocumentation), TimeSpan.FromDays(7));

    /// <summary>
    ///     Gets a predefined output cache policy for 1 minute.
    /// </summary>
    public static readonly OutputCachePolicy OneMinute = new(nameof(OneMinute), TimeSpan.FromMinutes(1));
}
