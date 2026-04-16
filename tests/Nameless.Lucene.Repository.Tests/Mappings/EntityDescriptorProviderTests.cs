using System.Reflection;
using Nameless.Lucene.Repository.Fixtures;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.DependencyInjection;

namespace Nameless.Lucene.Repository.Mappings;

[UnitTest]
public class EntityDescriptorProviderTests {
    [Fact]
    public void WhenGetDescriptor_WhenEntityMappingExists_ThenEntityDescriptor() {
        // arrange
        var expected = typeof(Car).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                  .Select(prop => prop.Name)
                                  .ToArray();
        var serviceProvider = new ServiceProviderMocker().WithGetService(CarEntityMapping.Instance)
                                                         .Build();
        var sut = new EntityDescriptorProvider(serviceProvider);

        // act
        var actual = sut.GetDescriptor<Car>();

        // assert
        Assert.Multiple(() => {
            Assert.IsType<EntityDescriptor<Car>>(actual);
            Assert.Equal(4, actual.Properties.Count);
            Assert.Equal(expected, actual.Properties.Select(prop => prop.Name));
        });
    }

    [Fact]
    public void WhenGetDescriptor_WhenEntityMappingExists_WhenEntityMappingDoNotMapID_ThenThrowsException() {
        // arrange
        var serviceProvider = new ServiceProviderMocker().WithGetService(MissingIDCarEntityMapping.Instance)
                                                         .Build();
        var sut = new EntityDescriptorProvider(serviceProvider);

        // act
        var actual = Record.Exception(sut.GetDescriptor<Car>);

        // assert
        Assert.IsType<MissingEntityIDException>(actual);
    }
}
