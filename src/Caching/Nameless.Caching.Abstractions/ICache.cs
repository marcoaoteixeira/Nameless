namespace Nameless.Caching;

/// <summary>
/// Provides means to deal with cache system.
/// </summary>
public interface ICache {
    /// <summary>
    /// Stores an entity into the underlying cache.
    /// </summary>
    /// <param name="key">The key to the entity.</param>
    /// <param name="value">The actual entity to be stored.</param>
    /// <param name="opts">The store options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if was able to store in underlying cache; otherwise <c>false</c>.</returns>
    Task<bool> SetAsync(string key, object value, CacheEntryOptions opts, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an entity from the underlying cache.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="key">The key for the entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The entity if found; otherwise <c>default</c>.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken);

    /// <summary>
    /// Removes an entity from the underlying cache.
    /// </summary>
    /// <param name="key">The key to the entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if was able to remove the entity; otherwise <c>false</c>.</returns>
    Task<bool> RemoveAsync(string key, CancellationToken cancellationToken);
}