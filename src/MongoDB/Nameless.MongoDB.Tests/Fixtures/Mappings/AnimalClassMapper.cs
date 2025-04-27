using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Nameless.MongoDB.Fixtures.Entities;

namespace Nameless.MongoDB.Fixtures.Mappings;

public class AnimalClassMapper : DocumentMapperBase {
    public override BsonClassMap CreateMap() =>
        new BsonClassMap<Animal>(mapper => {
            mapper
                .MapIdProperty(animal => animal.ID)
                .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));

            mapper
                .MapProperty(animal => animal.Name)
                .SetElementName("name");
            mapper
                .MapProperty(animal => animal.Species)
                .SetElementName("blablabla");
        });
}