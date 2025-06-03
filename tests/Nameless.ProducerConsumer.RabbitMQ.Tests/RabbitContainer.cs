using Testcontainers.RabbitMq;

namespace Nameless.ProducerConsumer.RabbitMQ;

public sealed class RabbitContainer : IDisposable, IAsyncDisposable {
    private bool _disposed;
    public RabbitMqContainer Instance { get; private set; }

    public RabbitContainer() {
        Instance = new RabbitMqBuilder()
                  .WithImage(image: "rabbitmq:4.1.0-alpine")
                  .WithName(name: "rabbitmq-test-container")
                  .WithHostname("localhost")
                  .WithPortBinding(5672, 5672)
                  .WithUsername(username: "guest")
                  .WithPassword(password: "guest")
                  .WithCleanUp(cleanUp: true)
                  .Build();

        Instance.StartAsync().Wait();
    }

    public void Dispose() {
        if (_disposed) { return; }

        DisposeAsync().AsTask().Wait();

        _disposed = true;
    }

    public async ValueTask DisposeAsync() {
        if (_disposed) { return; }

        await DisposeAsyncCore().ConfigureAwait(continueOnCapturedContext: false);

        _disposed = true;
    }

    private async ValueTask DisposeAsyncCore() {
        if (Instance is not null) {
            await Instance.StopAsync()
                          .ConfigureAwait(continueOnCapturedContext: false);
            await Instance.DisposeAsync()
                          .ConfigureAwait(continueOnCapturedContext: false);

            Instance = null;
        }
    }
}

// This class has no code, and is never created. Its purpose is simply to be
// the place to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
// See https://xunit.net/docs/shared-context for more information.
[CollectionDefinition(nameof(RabbitContainerCollection))]
public sealed class RabbitContainerCollection : ICollectionFixture<RabbitContainer>;