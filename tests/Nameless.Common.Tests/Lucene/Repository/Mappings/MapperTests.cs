using Lucene.Net.Documents;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Lucene.Repository.Mappings;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Lucene.Repository.Mappings;

[UnitTest]
public class MapperTests {
    // Dedicated entity type to avoid static-cache collisions with other test classes.
    private class MapperTestEntity {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    private class MapperTestEntityMapping : IEntityMapping<MapperTestEntity> {
        public void Map(IEntityDescriptor<MapperTestEntity> descriptor) {
            descriptor.SetID(e => e.Id);
            descriptor.SetProperty(e => e.Name, PropertyOptions.Store);
            descriptor.SetProperty(e => e.Age, PropertyOptions.Store);
        }
    }

    private static IMapper CreateMapper() {
        var services = new ServiceCollection();
        services.AddSingleton<IEntityMapping<MapperTestEntity>, MapperTestEntityMapping>();
        var provider = services.BuildServiceProvider();
        return new Mapper(new EntityDescriptorProvider(provider));
    }

    [Fact]
    public void Map_EntityToDocument_ContainsExpectedFields() {
        // Arrange
        var mapper = CreateMapper();
        var entity = new MapperTestEntity { Id = "1", Name = "Alice", Age = 30 };

        // Act
        var document = mapper.Map(entity);

        // Assert
        Assert.NotNull(document.GetField("Id"));
        Assert.NotNull(document.GetField("Name"));
        Assert.NotNull(document.GetField("Age"));
    }

    [Fact]
    public void Map_DocumentToEntity_SetsExpectedProperties() {
        // Arrange
        var mapper = CreateMapper();
        var document = new Document();
        document.Add(new StringField("Id", "42", Field.Store.YES));
        document.Add(new StringField("Name", "Bob", Field.Store.YES));
        document.Add(new Int32Field("Age", 25, Field.Store.YES));

        // Act
        var entity = mapper.Map<MapperTestEntity>(document);

        // Assert
        Assert.Equal("42", entity.Id);
        Assert.Equal("Bob", entity.Name);
        Assert.Equal(25, entity.Age);
    }

    [Fact]
    public void TryGetID_WithMappedId_ReturnsTrueAndValue() {
        // Arrange
        var mapper = CreateMapper();

        // Act
        var found = mapper.TryGetID<MapperTestEntity>(out var descriptor);

        // Assert
        Assert.True(found);
        Assert.NotNull(descriptor);
        Assert.Equal("Id", descriptor.Name);
        Assert.True(descriptor.IsID);
    }
}
