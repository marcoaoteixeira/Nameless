using Nameless.Lucene.Repository.Mappings;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Lucene.Repository.Mappings;

[UnitTest]
public class EntityDescriptorTests {
    private class TestEntity {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    [Fact]
    public void SetID_WithValidExpression_RegistersIdProperty() {
        // Arrange
        var sut = new EntityDescriptor<TestEntity>();

        // Act
        sut.SetID(e => e.Id);
        var idProperty = sut.Properties.SingleOrDefault(p => p.IsID);

        // Assert
        Assert.NotNull(idProperty);
        Assert.Equal("Id", idProperty.Name);
    }

    [Fact]
    public void SetProperty_WithValidExpression_RegistersProperty() {
        // Arrange
        var sut = new EntityDescriptor<TestEntity>();

        // Act
        sut.SetID(e => e.Id);
        sut.SetProperty(e => e.Name, PropertyOptions.Store);

        var property = sut.Properties.SingleOrDefault(p => p.Name == "Name");

        // Assert
        Assert.NotNull(property);
        Assert.False(property.IsID);
        Assert.Equal(PropertyOptions.Store, property.Options);
    }

    [Fact]
    public void Properties_AfterSetup_ContainsAllMappedProperties() {
        // Arrange
        var sut = new EntityDescriptor<TestEntity>();

        // Act
        sut.SetID(e => e.Id);
        sut.SetProperty(e => e.Name, PropertyOptions.Store);
        sut.SetProperty(e => e.Age, PropertyOptions.Store);

        // Assert
        Assert.Equal(3, sut.Properties.Count);
        Assert.Contains(sut.Properties, p => p.Name == "Id" && p.IsID);
        Assert.Contains(sut.Properties, p => p.Name == "Name");
        Assert.Contains(sut.Properties, p => p.Name == "Age");
    }

    [Fact]
    public void SetProperty_DuplicateName_OverwritesExistingEntry() {
        // Arrange — the backing store uses a Dictionary keyed by name,
        // so a second SetProperty call for the same property name replaces
        // the first registration rather than throwing.
        var sut = new EntityDescriptor<TestEntity>();
        sut.SetID(e => e.Id);
        sut.SetProperty(e => e.Name, PropertyOptions.None);

        // Act
        sut.SetProperty(e => e.Name, PropertyOptions.Store | PropertyOptions.Analyze);

        var nameProperty = sut.Properties.Single(p => p.Name == "Name");

        // Assert — only one entry exists and it has the updated options
        Assert.Equal(PropertyOptions.Store | PropertyOptions.Analyze, nameProperty.Options);
        Assert.Equal(2, sut.Properties.Count);
    }
}
