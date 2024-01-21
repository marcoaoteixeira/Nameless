using MongoDB.Bson.Serialization;
using Nameless.MongoDB.UnitTest.Fixtures.Entities;

namespace Nameless.MongoDB.UnitTest.Fixtures.Mappings {
    public class AnimalClassMapping : ClassMappingBase<Animal> {
        public override void Map(BsonClassMap<Animal> mapper) {
            mapper
                .MapIdProperty(animal => animal.ID);
            mapper
                .MapProperty(animal => animal.Name)
                .SetElementName("name");
            mapper
                .MapProperty(animal => animal.Species)
                .SetElementName("blablabla");
        }
    }
}
