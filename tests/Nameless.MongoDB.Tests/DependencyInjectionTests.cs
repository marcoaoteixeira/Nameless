using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Nameless.MongoDB.Fixtures.Mappings;

namespace Nameless.MongoDB;

public class DependencyInjectionTests {
    [Fact]
    public void Register_Resolve_MongoDB_Services() {
        // arrange
        var mongoDbOptionsLogger = Mock.Of<ILogger<MongoOptions>>();
        var services = new ServiceCollection();
        services.AddSingleton(mongoDbOptionsLogger);
        services.RegisterMongoServices(opts => {
            opts.DatabaseName = "sample";
            opts.DocumentMappers =
                [$"{typeof(AnimalClassMapper).FullName}, {typeof(AnimalClassMapper).Assembly.GetName().Name}"];
        });

        using var provider = services.BuildServiceProvider();

        // act
        var service = provider.GetService<IMongoCollectionProvider>();

        // assert
        Assert.That(service, Is.InstanceOf<MongoCollectionProvider>());
    }
}