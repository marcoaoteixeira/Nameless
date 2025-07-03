using Nameless.Localization.Json.Objects;
using Nameless.Null;

namespace Nameless.Localization.Json.Internals;

/// <summary>
/// Represents a cache entry for a resource.
/// </summary>
internal sealed record CacheEntry {
    /// <summary>
    /// An empty cache entry that contains an empty resource and a null disposable for the file change callback.
    /// </summary>
    internal static CacheEntry Empty => new(Resource.Empty, NullDisposable.Instance);

    /// <summary>
    /// Gets the resource associated with this cache entry.
    /// </summary>
    internal Resource Resource { get; }

    /// <summary>
    /// Gets the file change callback for this cache entry.
    /// </summary>
    internal IDisposable FileChangeCallback { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheEntry"/> class.
    /// </summary>
    /// <param name="resource">The resource.</param>
    /// <param name="fileChangeCallback">The file change callback.</param>
    internal CacheEntry(Resource resource, IDisposable fileChangeCallback) {
        Resource = resource;
        FileChangeCallback = fileChangeCallback;
    }
}