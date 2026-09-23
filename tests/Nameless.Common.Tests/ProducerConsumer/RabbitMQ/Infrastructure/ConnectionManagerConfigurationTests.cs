using System.Security.Authentication;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using Nameless.Testing.Tools.Mockers.Logging;
using RabbitMQ.Client.Exceptions;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

// Nothing listens on the chosen port, so connecting fails fast with
// BrokerUnreachableException after the factory was fully configured.
[IntegrationTest]
public class ConnectionManagerConfigurationTests {
    private static ConnectionManager CreateSut(ServerOptions server) {
        var options = Microsoft.Extensions.Options.Options.Create(new RabbitMQOptions { Server = server });
        var logger = new LoggerMocker<ConnectionManager>().WithAnyLogLevel().Build();

        return new ConnectionManager(options, logger);
    }

    private static ServerOptions Unreachable() => new() { Hostname = "127.0.0.1", Port = 1 };

    [Fact]
    public async Task GetConnectionAsync_WhenBrokerUnreachable_Throws() {
        // arrange
        await using var sut = CreateSut(Unreachable());

        // act & assert
        await Assert.ThrowsAsync<BrokerUnreachableException>(
            () => sut.GetConnectionAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetConnectionAsync_WithCredentialsSslAndCertificate_BuildsFactoryThenFailsToConnect() {
        // arrange
        var server = Unreachable() with {
            Username = "user",
            Password = "pass",
            Ssl = new SslOptions { Enabled = true, ServerName = "localhost", Protocol = SslProtocols.Tls12 },
            Certificate = new CertificateOptions { CertPath = "missing.pfx", CertPassword = "pwd" }
        };

        await using var sut = CreateSut(server);

        // act & assert
        await Assert.ThrowsAsync<BrokerUnreachableException>(
            () => sut.GetConnectionAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetConnectionAsync_CalledTwice_ReusesFactory() {
        // arrange
        await using var sut = CreateSut(Unreachable());

        // act & assert
        await Assert.ThrowsAsync<BrokerUnreachableException>(
            () => sut.GetConnectionAsync(TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<BrokerUnreachableException>(
            () => sut.GetConnectionAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetConnectionAsync_AfterDispose_ThrowsObjectDisposedException() {
        // arrange
        var sut = CreateSut(Unreachable());
        sut.Dispose();

        // act & assert
        await Assert.ThrowsAsync<ObjectDisposedException>(
            () => sut.GetConnectionAsync(TestContext.Current.CancellationToken));
    }
}
