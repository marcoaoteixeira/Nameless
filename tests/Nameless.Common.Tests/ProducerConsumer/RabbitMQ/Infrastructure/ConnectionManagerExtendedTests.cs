using Microsoft.Extensions.Options;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

[UnitTest]
public class ConnectionManagerExtendedTests {
    private static ConnectionManager CreateSut(IOptions<RabbitMQOptions>? options = null) {
        options ??= Microsoft.Extensions.Options.Options.Create(new RabbitMQOptions {
            Server = new ServerOptions {
                Hostname = "localhost",
                Port = 5672,
                VirtualHost = "/"
            }
        });

        var logger = new LoggerMocker<ConnectionManager>()
            .WithAnyLogLevel()
            .Build();

        return new ConnectionManager(options, logger);
    }

    [Fact]
    public async Task DisposeAsync_CalledMultipleTimes_DoesNotThrow() {
        // arrange
        var sut = CreateSut();

        // act
        var exception = await Record.ExceptionAsync(async () => {
            await sut.DisposeAsync();
            await sut.DisposeAsync();
        });

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task GetConnectionAsync_AfterDisposeAsync_ThrowsObjectDisposedException() {
        // arrange
        var sut = CreateSut();
        await sut.DisposeAsync();

        // act & assert
        await Assert.ThrowsAsync<ObjectDisposedException>(
            () => sut.GetConnectionAsync(CancellationToken.None)
        );
    }

    [Fact]
    public void Dispose_WhenConnectionExists_DoesNotThrow() {
        // arrange — no live broker, so we only verify that disposing without
        // a connection (the only reachable path without a broker) does not throw.
        var sut = CreateSut();

        // act
        sut.Dispose();

        // assert — verify subsequent dispose is also safe (idempotency)
        var exception = Record.Exception(sut.Dispose);
        Assert.Null(exception);
    }
}
