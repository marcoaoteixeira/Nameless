namespace Nameless.MongoDB.Infrastructure;

/// <summary>
/// Extensions for <see cref="ICollectionNamingStrategy"/>.
/// </summary>
public static class CollectionNamingStrategyExtensions {
    /// <summary>
    /// Gets the name of the MongoDB collection for a given type.
    /// </summary>
    /// <typeparam name="T">Type of the collection.</typeparam>
    /// <param name="self">The current <see cref="ICollectionNamingStrategy"/>.</param>
    /// <returns>
    /// The name of the MongoDB collection as a string.
    /// </returns>
    public static string GetCollectionName<T>(this ICollectionNamingStrategy self) {
        return self.GetCollectionName(typeof(T));
    }
}