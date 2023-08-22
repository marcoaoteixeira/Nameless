using MongoDB.Driver;

namespace Nameless.MongoDB {
    public interface IMongoCollectionProvider {
        #region Methods

        IMongoCollection<T> GetCollection<T>(string? name = null, MongoCollectionSettings? settings = null);

        #endregion
    }
}
