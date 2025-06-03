using MongoDB.Driver;

namespace Nameless.MongoDB;

/// <summary>
///     Provides infrastructure to deal with MongoDatabase objects.
/// </summary>
public interface IMongoDatabaseManager {
    /// <summary>
    ///     Retrieves a <see cref="IMongoDatabase" /> instance to manipulate
    ///     collection of documents.
    /// </summary>
    /// <param name="database">The name of the database</param>
    /// <returns>
    ///     An instance of <see cref="IMongoDatabase" />.
    /// </returns>
    IMongoDatabase GetDatabase(string database);
}