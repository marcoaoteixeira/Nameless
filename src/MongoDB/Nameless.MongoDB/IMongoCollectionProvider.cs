using MongoDB.Driver;

namespace Nameless.MongoDB {
    public interface IMongoCollectionProvider {
        #region Methods

        /// <summary>
        /// Gets the current database associated to this provider.
        /// </summary>
        string DatabaseName { get; }
        /// <summary>
        /// Retrieves the specified collection.
        /// </summary>
        /// <typeparam name="T">Type of the collection.</typeparam>
        /// <param name="name">
        /// If the <paramref name="name"/> is not defined, then the collection
        /// naming strategy will take care of it.
        /// </param>
        /// <param name="settings">The collection settings.</param>
        /// <returns>The collection.</returns>
        IMongoCollection<T> GetCollection<T>(string name, MongoCollectionSettings settings);

        #endregion
    }
}
