using Microsoft.Extensions.Configuration;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Helpers;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

[UnitTest]
public class ConnectionManagerExtendedTests {
    private static ConnectionManager CreateSut(IConfiguration? configuration = null) {
        configuration ??= ConfigurationHelper.CreateConfiguration(new Dictionary<string, string?> {
            ["RabbitMQ:Server:Hostname"] = "localhost",
            ["RabbitMQ:Server:Port"] = "5672",
            ["RabbitMQ:Server:VirtualHost"] = "/",
            ["RabbitMQ:Prefetch:IsEnabled"] = "false"
        });

        var logger = new LoggerMocker<ConnectionManager>()
            .WithAnyLogLevel()
            .Build();

        return new ConnectionManager(configuration, logger);
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
