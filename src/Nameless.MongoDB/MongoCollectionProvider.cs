using MongoDB.Driver;

namespace Nameless.MongoDB;

/// <summary>
///     Default implementation of <see cref="IMongoCollectionProvider"/>.
/// </summary>
public sealed class MongoCollectionProvider : IMongoCollectionProvider {
    private readonly ICollectionNamingStrategy _collectionNamingStrategy;
    private readonly IMongoDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoCollectionProvider"/>.
    /// </summary>
    /// <param name="database">The Mongo database.</param>
    /// <param name="collectionNamingStrategy">The collection naming strategy.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="database"/> or
    ///     <paramref name="collectionNamingStrategy"/> is <see langword="null"/>.
    /// </exception>
    public MongoCollectionProvider(IMongoDatabase database, ICollectionNamingStrategy collectionNamingStrategy) {
        _database = Prevent.Argument.Null(database);
        _collectionNamingStrategy = Prevent.Argument.Null(collectionNamingStrategy);
    }

    /// <inheritdoc />
    public string DatabaseName => _database.DatabaseNamespace.DatabaseName;

    /// <inheritdoc />
    public IMongoCollection<T> GetCollection<T>(string name, MongoCollectionSettings? settings) {
        var collectionName = string.IsNullOrWhiteSpace(name)
            ? _collectionNamingStrategy.GetCollectionName<T>()
            : name;

        return _database.GetCollection<T>(collectionName, settings);
    }
}