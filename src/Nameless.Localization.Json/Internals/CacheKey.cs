using System.Diagnostics;

namespace Nameless.Localization.Json.Internals;

/// <summary>
/// Represents a cache key for a resource.
/// </summary>
[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
internal readonly record struct CacheKey {
    /// <summary>
    /// Gets the base name of the resource.
    /// </summary>
    internal string BaseName { get; }

    /// <summary>
    /// Gets the location of the resource.
    /// </summary>
    internal string Location { get; }

    /// <summary>
    /// Gets the culture of the resource.
    /// </summary>
    internal string Culture { get; }

    /// <summary>
    /// Gets the path of the resource.
    /// </summary>
    internal string Path => $"{BaseName}.{Location}";

    private string DebuggerDisplayValue
        => $"{Culture}: {BaseName}.{Location}";

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheKey"/>.
    /// </summary>
    /// <param name="baseName">The base name.</param>
    /// <param name="location">The location.</param>
    /// <param name="culture">The culture.</param>
    internal CacheKey(string baseName, string location, string culture) {
        BaseName = baseName;
        Location = location;
        Culture = culture;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="CacheKey"/>.
    /// </summary>
    /// <param name="baseName">The base name.</param>
    /// <param name="location">The location.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>
    /// A new instance of the <see cref="CacheKey"/>.
    /// </returns>
    internal static CacheKey Create(string baseName, string location, string culture) {
        return new CacheKey(baseName, location, culture);
    }
}