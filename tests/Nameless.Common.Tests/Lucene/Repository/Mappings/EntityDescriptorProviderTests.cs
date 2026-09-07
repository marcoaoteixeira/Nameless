using Microsoft.Extensions.DependencyInjection;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Lucene.Repository.Mappings;

[UnitTest]
public class EntityDescriptorProviderTests {
    // Dedicated entity type per test class so the static cache does not
    // carry state across other test classes that use TestEntity.
    private class ProviderTestEntity {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    private class ProviderTestEntityMapping : IEntityMapping<ProviderTestEntity> {
        public void Map(IEntityDescriptor<ProviderTestEntity> descriptor) {
            descriptor.SetID(e => e.Id);
            descriptor.SetProperty(e => e.Name, PropertyOptions.Store);
        }
    }

    private static IServiceProvider BuildServiceProvider() {
        var services = new ServiceCollection();
        services.AddSingleton<IEntityMapping<ProviderTestEntity>, ProviderTestEntityMapping>();
        return services.BuildServiceProvider();
    }

    [Fact]
    public void GetDescriptor_FirstCall_ReturnsDescriptor() {
        // Arrange
        var sut = new EntityDescriptorProvider(BuildServiceProvider());

        // Act
        var descriptor = sut.GetDescriptor<ProviderTestEntity>();

        // Assert
        Assert.NotNull(descriptor);
        Assert.Contains(descriptor.Properties, p => p.IsID && p.Name == "Id");
    }

    [Fact]
    public void GetDescriptor_SecondCall_ReturnsCachedSameInstance() {
        // Arrange
        var sut = new EntityDescriptorProvider(BuildServiceProvider());

        // Act
        var first = sut.GetDescriptor<ProviderTestEntity>();
        var second = sut.GetDescriptor<ProviderTestEntity>();

        // Assert — static cache guarantees reference equality across calls
        Assert.Same(first, second);
    }
}
