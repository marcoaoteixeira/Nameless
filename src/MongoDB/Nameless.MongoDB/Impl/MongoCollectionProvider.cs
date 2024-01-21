using MongoDB.Driver;

namespace Nameless.MongoDB.Impl {
    public sealed class MongoCollectionProvider : IMongoCollectionProvider {
        #region Private Read-Only Fields

        private readonly IMongoDatabase _database;
        private readonly ICollectionNamingStrategy _collectionNamingStrategy;

        #endregion

        #region Public Constructors

        public MongoCollectionProvider(IMongoDatabase database)
            : this(database, CollectionNamingStrategy.Instance) { }

        public MongoCollectionProvider(IMongoDatabase database, ICollectionNamingStrategy collectionNamingStrategy) {
            _database = Guard.Against.Null(database, nameof(database));
            _collectionNamingStrategy = collectionNamingStrategy ?? CollectionNamingStrategy.Instance;
        }

        #endregion

        #region IMongoCollectionProvider Members

        /// <inheritdoc/>
        public string DatabaseName => _database.DatabaseNamespace.DatabaseName;

        /// <inheritdoc/>
        public IMongoCollection<T> GetCollection<T>(string? name, MongoCollectionSettings? settings)
            => _database.GetCollection<T>(
                name: string.IsNullOrWhiteSpace(name)
                    ? _collectionNamingStrategy.GetCollectionName<T>()
                    : name,
                settings: settings
            );

        #endregion
    }
}
