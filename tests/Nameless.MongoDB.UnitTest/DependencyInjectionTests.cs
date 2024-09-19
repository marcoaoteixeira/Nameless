using Microsoft.Extensions.DependencyInjection;
using Nameless.MongoDB.Impl;

namespace Nameless.MongoDB.UnitTest;

public class DependencyInjectionTests {
    [Category(Categories.RUNS_ON_DEV_MACHINE)]
    [Test(Description = "You'll need a local mongo instance or configure it to access a remote instance. See README file.")]
    public void Register_Resolve_MongoDB_Services() {
        // arrange
        var services = new ServiceCollection();
        services.AddMongoDB(opts => opts.Database = "local");

        using var provider = services.BuildServiceProvider();

        // act
        var service = provider.GetService<IMongoCollectionProvider>();

        // assert
        Assert.That(service, Is.InstanceOf<MongoCollectionProvider>());
    }
}