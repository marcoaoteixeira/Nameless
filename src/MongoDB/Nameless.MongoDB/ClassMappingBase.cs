using MongoDB.Bson.Serialization;

namespace Nameless.MongoDB {
    public abstract class ClassMappingBase<TDocument> {
        #region Public Abstract Methods

        public abstract void Map(BsonClassMap<TDocument> mapper);

        #endregion
    }
}
