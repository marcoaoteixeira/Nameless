using Microsoft.Extensions.DependencyInjection;
using Nameless.MongoDB.Fixtures.Entities;
using Nameless.MongoDB.Fixtures.Mappings;

namespace Nameless.MongoDB;

public class DependencyInjectionTests {
    [Category(Categories.RUNS_ON_DEV_MACHINE)]
    [Test(Description = "You'll need a local mongo instance or configure it to access a remote instance. See README file.")]
    public void Register_Resolve_MongoDB_Services() {
        // arrange
        var services = new ServiceCollection();
        services.AddMongoDB(opts => {
            opts.DatabaseName = "sample";
            opts.DocumentMappers = [$"{typeof(AnimalClassMapper).FullName}, {typeof(AnimalClassMapper).Assembly.GetName().Name}"];
        });

        using var provider = services.BuildServiceProvider();

        // act
        var service = provider.GetService<IMongoCollectionProvider>();

        var collection = service.GetCollection<Animal>(name: string.Empty, settings: null);

        collection.InsertOne(new Animal {
            ID = Guid.NewGuid(),
            Name = "Bolota",
            Species = "Dog"
        });
        
        // assert
        Assert.That(service, Is.InstanceOf<MongoCollectionProvider>());
    }
}