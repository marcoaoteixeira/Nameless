using MongoDB.Driver;

namespace Nameless.MongoDB;

/// <summary>
///     Provides access to MongoDB collections.
/// </summary>
public interface IMongoCollectionProvider {
    /// <summary>
    ///     Gets the current database associated to this provider.
    /// </summary>
    string DatabaseName { get; }

    /// <summary>
    ///     Retrieves the specified collection.
    /// </summary>
    /// <typeparam name="T">Type of the collection.</typeparam>
    /// <param name="name">
    ///     If the <paramref name="name" /> is not defined, then the collection
    ///     naming strategy will take care of it.
    /// </param>
    /// <param name="settings">The collection settings.</param>
    /// <returns>
    ///     A <see cref="IMongoCollection{T}" /> representing the specified collection.
    /// </returns>
    IMongoCollection<T> GetCollection<T>(string name, MongoCollectionSettings? settings);
}