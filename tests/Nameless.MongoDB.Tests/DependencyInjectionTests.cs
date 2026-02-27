using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.MongoDB;

[IntegrationTest]
[Collection(nameof(MongoContainerCollectionFixture))]
public class DependencyInjectionTests {
    [Fact]
    public void Register_Resolve_MongoDB_Services() {
        // arrange
        var mongoDbOptionsLogger = Mock.Of<ILogger<MongoOptions>>();
        var services = new ServiceCollection();
        services.AddSingleton(mongoDbOptionsLogger);
        services.RegisterMongo(
            registration: reg => reg.IncludeAssemblies(typeof(DependencyInjectionTests).Assembly),
            configure: opts => opts.DatabaseName = "sample"
        );

        using var provider = services.BuildServiceProvider();

        // act
        var service = provider.GetService<IMongoCollectionProvider>();

        // assert
        Assert.IsType<MongoCollectionProvider>(service);
    }
}