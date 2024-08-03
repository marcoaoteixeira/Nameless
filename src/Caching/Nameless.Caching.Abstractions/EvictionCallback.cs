namespace Nameless.Caching {
    /// <summary>
    /// Delegate for eviction callback.
    /// </summary>
    /// <param name="key">The key of the entity in the cache.</param>
    /// <param name="value">The entity that was evicted.</param>
    /// <param name="reason">The reason of the eviction.</param>
    public delegate void EvictionCallback(
        string key,
        object? value = null,
        string? reason = null
    );
}
