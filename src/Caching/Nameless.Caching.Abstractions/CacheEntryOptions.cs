namespace Nameless.Caching;

/// <summary>
/// Provides information for cache entries.
/// </summary>
public sealed record CacheEntryOptions {
    /// <summary>
    /// Gets the <see cref="TimeSpan"/> for expiration.
    /// </summary>
    public TimeSpan ExpiresIn { get; }

    /// <summary>
    /// Gets the callback for eviction action.
    /// </summary>
    public EvictionCallback? EvictionCallback { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="CacheEntryOptions"/>
    /// </summary>
    public CacheEntryOptions() {
        ExpiresIn = TimeSpan.Zero;
        EvictionCallback = null;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CacheEntryOptions"/>
    /// </summary>
    /// <param name="expiresIn">The <see cref="TimeSpan"/> representing the expiration time.</param>
    public CacheEntryOptions(TimeSpan expiresIn) {
        ExpiresIn = expiresIn;
        EvictionCallback = null;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CacheEntryOptions"/>
    /// </summary>
    /// <param name="expiresIn">The <see cref="TimeSpan"/> representing the expiration time.</param>
    /// <param name="evictionCallback">The callback action when eviction occurs.</param>
    public CacheEntryOptions(TimeSpan expiresIn, EvictionCallback evictionCallback) {
        ExpiresIn = expiresIn;
        EvictionCallback = evictionCallback;
    }
}