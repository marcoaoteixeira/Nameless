using System.Collections.ObjectModel;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public sealed class RabbitContainer : IAsyncLifetime {
    private const string HOST_NAME = "localhost";
    private const int CONTAINER_PORT = 5672;
    private const string USERNAME = "guest";
    private const string PASSWORD = "guest";

    public const int HOST_PORT = 15672;

    private static readonly ReadOnlyDictionary<string, string> Parameter = new(new Dictionary<string, string> {
        { "RABBITMQ_DEFAULT_USER", USERNAME },
        { "RABBITMQ_DEFAULT_PASS", PASSWORD }
    });

    private readonly IContainer _container = new ContainerBuilder().WithImage("rabbitmq:4.1.0-alpine")
                                                                   .WithName("rabbitmq-test-container")
                                                                   .WithPortBinding(HOST_PORT, CONTAINER_PORT)
                                                                   .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(CONTAINER_PORT))
                                                                   .WithHostname(HOST_NAME)
                                                                   .WithCleanUp(cleanUp: true)
                                                                   .WithEnvironment(Parameter)
                                                                   .Build();

    private IConnection _connection;

    public async ValueTask InitializeAsync() {
        await _container.StartAsync();
    }

    public async ValueTask DisposeAsync() {
        if (_connection is not null) {
            await _connection.DisposeAsync();
        }

        if (_container is not null) {
            await _container.StopAsync();
            await _container.DisposeAsync();
        }
    }

    public async Task<IConnection> GetDefaultConnectionAsync() {
        if (_connection is not null) {
            return _connection;
        }

        var configurationFactory = new ConnectionFactory {
            HostName = HOST_NAME,
            Port = HOST_PORT,
            UserName = USERNAME,
            Password = PASSWORD
        };

        _connection = await configurationFactory.CreateConnectionAsync(CancellationToken.None);

        return _connection;
    }
}

// This class has no code, and is never created. Its purpose is simply to be
// the place to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
// See https://xunit.net/docs/shared-context for more information.
[CollectionDefinition(nameof(RabbitContainerCollection))]
public sealed class RabbitContainerCollection : ICollectionFixture<RabbitContainer>;