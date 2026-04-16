#pragma warning disable CA1816

using Nameless.Lucene.ObjectModel;
using Nameless.Lucene.Repository.Fixtures;
using Nameless.Lucene.Repository.Mappings;
using Nameless.Lucene.Repository.Requests;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.DependencyInjection;
using Nameless.Testing.Tools.Mockers.Lucene;

namespace Nameless.Lucene.Repository;

[IntegrationTest]
public class RepositoryImplTests {
    private static RepositoryImpl CreateSut(IIndex index) {
        var analyzerProvider = new AnalyzerProviderMocker().WithGetAnalyzer().Build();
        var indexProvider = new IndexProviderMocker().WithGet(index).Build();
        var serviceProvider = new ServiceProviderMocker().WithGetService<IEntityMapping<Car>>(
            new CarEntityMapping()
        ).Build();
        var entityDescriptorProvider = new EntityDescriptorProvider(serviceProvider);
        var mapper = new Mapper(entityDescriptorProvider);

        return new RepositoryImpl(analyzerProvider, indexProvider, mapper);
    }

    [Fact]
    public async Task WhenInsertAsync_ThenIndexShouldStoreEntities() {
        // arrange
        var documents = new DocumentCollection();
        var car = CarFaker.Instance.Generate();

        var index = new IndexMocker()
                    .WithInsert(callback: UpdateDocuments)
                    .WithSaveChanges()
                    .Build();
        var sut = CreateSut(index);

        // act
        var request = new InsertEntitiesRequest<Car>(Entities: [car]);
        var actual = await sut.InsertAsync(request, TestContext.Current.CancellationToken);

        // assert
        Assert.Multiple(() => {
            Assert.True(actual.Success);
            Assert.Equal(1, actual.Value.Count);
            Assert.Single(documents);

            var document = documents.Single();

            Assert.Equal(car.ID.ToString(), document.Fields[0].GetStringValue());
            Assert.Equal(car.Brand, document.Fields[1].GetStringValue());
            Assert.Equal(car.Model, document.Fields[2].GetStringValue());
            Assert.Equal(car.Year, document.Fields[3].GetInt32Value());
        });

        void UpdateDocuments(DocumentCollection arg) {
            documents = arg;
        }
    }
}
