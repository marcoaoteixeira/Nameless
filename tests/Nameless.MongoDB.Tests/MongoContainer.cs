using System.Collections.ObjectModel;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Nameless.MongoDB;

// This class has no code, and is never created. Its purpose is simply to be
// the place to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
// See https://xunit.net/docs/shared-context for more information.
[CollectionDefinition(nameof(MongoContainerCollectionFixture))]
public sealed class MongoContainerCollectionFixture : ICollectionFixture<MongoContainer>;

public sealed class MongoContainer : IAsyncLifetime {
    private const string HOST_NAME = "localhost";
    private const int CONTAINER_PORT = 27017;
    private const string USERNAME = "root";
    private const string PASSWORD = "root";

    public const int HOST_PORT = 27017;

    private static readonly ReadOnlyDictionary<string, string> Parameter = new(new Dictionary<string, string> {
        { "MONGO_INITDB_ROOT_USERNAME", USERNAME },
        { "MONGO_INITDB_ROOT_PASSWORD", PASSWORD }
    });

    private readonly IContainer _container = new ContainerBuilder("mongo:8.2.3")
                                             .WithName(name: "mongo-test-container")
                                             .WithPortBinding(HOST_PORT, CONTAINER_PORT)
                                             .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(CONTAINER_PORT))
                                             .WithHostname(HOST_NAME)
                                             .WithCleanUp(cleanUp: true)
                                             .WithEnvironment(Parameter)
                                             .Build();

    public async ValueTask InitializeAsync() {
        await _container.StartAsync();
    }

    public async ValueTask DisposeAsync() {
        if (_container is not null) {
            await _container.StopAsync();
            await _container.DisposeAsync();
        }
    }
}
