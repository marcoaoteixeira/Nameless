using Microsoft.Extensions.DependencyInjection;
using Nameless.MongoDB.Fixtures.Mappings;

namespace Nameless.MongoDB;

public class DependencyInjectionTests {
    [Test]
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

        // assert
        Assert.That(service, Is.InstanceOf<MongoCollectionProvider>());
    }
}