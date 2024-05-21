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
            _collectionNamingStrategy = Guard.Against.Null(collectionNamingStrategy, nameof(collectionNamingStrategy));
        }

        #endregion

        #region IMongoCollectionProvider Members

        /// <inheritdoc/>
        public string DatabaseName => _database.DatabaseNamespace.DatabaseName;

        /// <inheritdoc/>
        public IMongoCollection<T> GetCollection<T>(string? name = null, MongoCollectionSettings? settings = null)
            => _database.GetCollection<T>(name: string.IsNullOrWhiteSpace(name)
                                            ? _collectionNamingStrategy.GetCollectionName<T>()
                                            : name,
                                          settings: settings
            );

        #endregion
    }
}
