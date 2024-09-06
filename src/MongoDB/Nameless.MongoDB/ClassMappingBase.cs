using MongoDB.Bson.Serialization;

namespace Nameless.MongoDB;

public abstract class ClassMappingBase<TDocument> {
    public abstract void Map(BsonClassMap<TDocument> mapper);
}