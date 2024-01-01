using MongoDB.Driver;

namespace Nameless.MongoDB.Impl {
    public sealed class MongoCollectionProvider : IMongoCollectionProvider {
        #region Private Read-Only Fields

        private readonly IMongoDatabase _database;
        private readonly ICollectionNamingStrategy _collectionNamingStrategy;

        #endregion

        #region Public Constructors

        public MongoCollectionProvider(IMongoDatabase database, ICollectionNamingStrategy collectionNamingStrategy) {
            _database = Guard.Against.Null(database, nameof(database));
            _collectionNamingStrategy = collectionNamingStrategy ?? CollectionNamingStrategy.Instance;
        }

        #endregion

        #region IMongoCollectionManager Members

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
        public IMongoCollection<T> GetCollection<T>(string? name = null, MongoCollectionSettings? settings = null)
            => _database.GetCollection<T>(
                name: string.IsNullOrWhiteSpace(name)
                    ? _collectionNamingStrategy.GetCollectionName<T>()
                    : name,
                settings: settings
            );

        #endregion
    }
}
