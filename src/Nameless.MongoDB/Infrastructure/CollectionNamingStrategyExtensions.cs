namespace Nameless.MongoDB.Infrastructure;

/// <summary>
/// Extensions for <see cref="ICollectionNamingStrategy"/>.
/// </summary>
public static class CollectionNamingStrategyExtensions {
    /// <param name="self">The current <see cref="ICollectionNamingStrategy"/>.</param>
    extension(ICollectionNamingStrategy self) {
        /// <summary>
        /// Gets the name of the MongoDB collection for a given type.
        /// </summary>
        /// <typeparam name="T">Type of the collection.</typeparam>
        /// <returns>
        /// The name of the MongoDB collection as a string.
        /// </returns>
        public string GetCollectionName<T>() {
            return self.GetCollectionName(typeof(T));
        }
    }
}