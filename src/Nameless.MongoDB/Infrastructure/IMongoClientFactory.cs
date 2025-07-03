using MongoDB.Driver;

namespace Nameless.MongoDB.Infrastructure;

public interface IMongoClientFactory {
    /// <summary>
    ///     Creates a new instance of <see cref="IMongoClient"/>.
    /// </summary>
    /// <returns>
    ///     A new instance of <see cref="IMongoClient"/>.
    /// </returns>
    IMongoClient CreateClient();
}