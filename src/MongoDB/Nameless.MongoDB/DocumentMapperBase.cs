using MongoDB.Bson.Serialization;

namespace Nameless.MongoDB;

public abstract class DocumentMapperBase {
    public abstract BsonClassMap CreateMap();
}