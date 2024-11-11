using MongoDB.Driver;

namespace Nameless.MongoDB;

public sealed class MongoCollectionProvider : IMongoCollectionProvider {
    private readonly IMongoDatabase _database;
    private readonly ICollectionNamingStrategy _collectionNamingStrategy;

    public MongoCollectionProvider(IMongoDatabase database, ICollectionNamingStrategy collectionNamingStrategy) {
        _database = Prevent.Argument.Null(database);
        _collectionNamingStrategy = Prevent.Argument.Null(collectionNamingStrategy);
    }

    /// <inheritdoc/>
    public string DatabaseName => _database.DatabaseNamespace.DatabaseName;

    /// <inheritdoc/>
    public IMongoCollection<T> GetCollection<T>(string name, MongoCollectionSettings? settings)
        => _database.GetCollection<T>(name: string.IsNullOrWhiteSpace(name)
                                          ? _collectionNamingStrategy.GetCollectionName<T>()
                                          : name,
                                      settings: settings
        );
}