using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Nameless.MongoDB.Fixtures.Entities;

namespace Nameless.MongoDB.Fixtures.Mappings;

public class AnimalClassMapping : IDocumentMapping {
    public BsonClassMap CreateMap() {
        return new BsonClassMap<Animal>(mapper => {
            mapper
                .MapIdProperty(animal => animal.ID)
                .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));

            mapper
                .MapProperty(animal => animal.Name)
                .SetElementName(elementName: "name");
            mapper
                .MapProperty(animal => animal.Species)
                .SetElementName(elementName: "blablabla");
        });
    }
}