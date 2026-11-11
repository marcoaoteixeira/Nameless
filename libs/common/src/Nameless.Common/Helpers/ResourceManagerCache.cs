using System.Collections.Concurrent;
using System.Globalization;
using System.Resources;

namespace Nameless.Helpers;

/// <summary>
///     Resource Manager Cache
/// </summary>
public static class ResourceManagerCache {
    private static readonly ConcurrentDictionary<Type, ResourceManager> Cache = new();

    /// <summary>
    ///     Looks up a resource value for a particular name.
    /// </summary>
    /// <remarks>
    ///     Looks in the specified CultureInfo, and if not found, all parent
    ///     CultureInfos. Returns null if the resource wasn't found.
    /// </remarks>
    /// <param name="resourceType">
    ///     Type of the resource.
    /// </param>
    /// <param name="resourceName">
    ///     The resource name.
    /// </param>
    /// <param name="culture">
    ///     The culture.
    /// </param>
    /// <returns>
    ///     The resource value if found; otherwise, <see langword="null"/>.
    /// </returns>
    public static string? GetString(Type resourceType, string resourceName, CultureInfo? culture = null) {
        var manager = Cache.GetOrAdd(resourceType, key => new ResourceManager(key));

        return manager.GetString(resourceName, culture);
    }

    /// <summary>
    ///     Looks up a resource value for a particular name.
    /// </summary>
    /// <param name="resourceType">
    ///     Type of the resource.
    /// </param>
    /// <param name="resourceName">
    ///     Name of the resource.
    /// </param>
    /// <param name="culture">
    ///     If <paramref name="culture"/> is <see langword="null"/> then looks
    ///     in the current thread's <see cref="CultureInfo"/>, and if not
    ///     found, all parent <see cref="CultureInfo"/>.
    /// </param>
    /// <returns>
    ///     If found, the resource as a stream; otherwise,
    ///     <see langword="null"/>.
    /// </returns>
    public static Stream? GetStream(Type resourceType, string resourceName, CultureInfo? culture = null) {
        var manager = Cache.GetOrAdd(resourceType, key => new ResourceManager(key));

        return manager.GetStream(resourceName, culture);
    }
}

/// <summary>
///     Resource Manager Cache Extension Methods
/// </summary>
public static class ResourceManagerCacheExtensions {
    extension(ResourceManagerCache) {
        /// <summary>
        ///     Looks up a resource value for a particular name.
        /// </summary>
        /// <remarks>
        ///     Looks in the specified CultureInfo, and if not found, all parent
        ///     CultureInfos. Returns null if the resource wasn't found.
        /// </remarks>
        /// <typeparam name="TResource">
        ///     Type of the resource.
        /// </typeparam>
        /// <param name="resourceName">
        ///     The resource name.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        ///     The resource value if found; otherwise, <see langword="null"/>.
        /// </returns>
        public static string? GetString<TResource>(string resourceName, CultureInfo? culture = null) {
            return ResourceManagerCache.GetString(typeof(TResource), resourceName, culture);
        }
    }

    /// <summary>
    ///     Looks up a resource value for a particular name.
    /// </summary>
    /// <typeparam name="TResource">
    ///     Type of the resource.
    /// </typeparam>
    /// <param name="resourceName">
    ///     Name of the resource.
    /// </param>
    /// <param name="culture">
    ///     If <paramref name="culture"/> is <see langword="null"/> then looks
    ///     in the current thread's <see cref="CultureInfo"/>, and if not
    ///     found, all parent <see cref="CultureInfo"/>.
    /// </param>
    /// <returns>
    ///     If found, the resource as a stream; otherwise,
    ///     <see langword="null"/>.
    /// </returns>
    public static Stream? GetStream<TResource>(string resourceName, CultureInfo? culture = null) {
        return ResourceManagerCache.GetStream(typeof(TResource), resourceName, culture);
    }
}