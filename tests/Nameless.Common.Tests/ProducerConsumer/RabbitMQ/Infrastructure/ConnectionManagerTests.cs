using Microsoft.Extensions.Configuration;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Helpers;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

public class ConnectionManagerTests {
    // ConnectionFactory is created internally by ConnectionManager (not injected),
    // so we cannot mock the actual RabbitMQ broker call. The tests here focus on
    // the disposal lifecycle and configuration wiring, which can be exercised
    // without a live broker.

    private static ConnectionManager CreateSut(IConfiguration? configuration = null) {
        configuration ??= CreateValidConfiguration();

        var logger = new LoggerMocker<ConnectionManager>()
            .WithAnyLogLevel()
            .Build();

        return new ConnectionManager(configuration, logger);
    }

    private static IConfiguration CreateValidConfiguration() {
        return ConfigurationHelper.CreateConfiguration(new Dictionary<string, string?> {
            ["RabbitMQ:Server:Hostname"] = "localhost",
            ["RabbitMQ:Server:Port"] = "5672",
            ["RabbitMQ:Server:VirtualHost"] = "/",
            ["RabbitMQ:Prefetch:IsEnabled"] = "false"
        });
    }

    [Fact]
    [UnitTest]
    public void Dispose_WhenNoConnectionWasCreated_DoesNotThrow() {
        // arrange
        var sut = CreateSut();

        // act
        var exception = Record.Exception(sut.Dispose);

        // assert
        Assert.Null(exception);
    }

    [Fact]
    [UnitTest]
    public async Task DisposeAsync_WhenNoConnectionWasCreated_DoesNotThrow() {
        // arrange
        var sut = CreateSut();

        // act
        var exception = await Record.ExceptionAsync(() => sut.DisposeAsync().AsTask());

        // assert
        Assert.Null(exception);
    }

    [Fact]
    [UnitTest]
    public void Dispose_CalledMultipleTimes_DoesNotThrow() {
        // arrange
        var sut = CreateSut();

        // act
        var exception = Record.Exception(() => {
            sut.Dispose();
            sut.Dispose();
        });

        // assert
        Assert.Null(exception);
    }

    [Fact]
    [UnitTest]
    public async Task GetConnectionAsync_AfterDispose_ThrowsObjectDisposedException() {
        // arrange
        var sut = CreateSut();
        sut.Dispose();

        // act & assert
        await Assert.ThrowsAsync<ObjectDisposedException>(
            () => sut.GetConnectionAsync(CancellationToken.None)
        );
    }
}
