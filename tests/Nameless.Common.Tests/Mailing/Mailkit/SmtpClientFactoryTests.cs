using System.Net;
using System.Net.Sockets;
using System.Text;
using MailKit.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Nameless.Mailing.Mailkit;

[IntegrationTest]
public class SmtpClientFactoryTests {
    private sealed class FakeSmtpServer : IAsyncDisposable {
        private readonly TcpListener _listener = new(IPAddress.Loopback, 0);
        private readonly bool _advertiseAuth;
        private readonly Task _loop;

        public List<string> Commands { get; } = [];
        public int Port => ((IPEndPoint)_listener.LocalEndpoint).Port;

        public FakeSmtpServer(bool advertiseAuth) {
            _advertiseAuth = advertiseAuth;
            _listener.Start();
            _loop = Task.Run(RunAsync);
        }

        private async Task RunAsync() {
            try {
                using var client = await _listener.AcceptTcpClientAsync();
                await using var stream = client.GetStream();
                using var reader = new StreamReader(stream, Encoding.ASCII);
                await using var writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true, NewLine = "\r\n" };

                await writer.WriteLineAsync("220 fake ESMTP");

                while (await reader.ReadLineAsync() is { } line) {
                    lock (Commands) { Commands.Add(line); }

                    if (line.StartsWith("EHLO", StringComparison.OrdinalIgnoreCase)) {
                        if (_advertiseAuth) {
                            await writer.WriteLineAsync("250-fake");
                            await writer.WriteLineAsync("250 AUTH PLAIN");
                        }
                        else {
                            await writer.WriteLineAsync("250 fake");
                        }
                    }
                    else if (line.StartsWith("AUTH", StringComparison.OrdinalIgnoreCase)) {
                        await writer.WriteLineAsync("235 2.7.0 Authentication successful");
                    }
                    else if (line.StartsWith("QUIT", StringComparison.OrdinalIgnoreCase)) {
                        await writer.WriteLineAsync("221 bye");
                        break;
                    }
                    else {
                        await writer.WriteLineAsync("250 OK");
                    }
                }
            }
            catch (Exception) {
                // server is torn down by the test; nothing to report
            }
        }

        public async ValueTask DisposeAsync() {
            _listener.Stop();
            await _loop;
        }
    }

    private static SmtpClientFactory CreateSut(int port, string? username = null, string? password = null)
        => new(Options.Create(new MailingOptions {
            Host = "127.0.0.1", Port = port, SecureSocket = SecureSocketOptions.None,
            Username = username, Password = password
        }));

    [Fact]
    public async Task CreateAsync_ConnectsToConfiguredServer() {
        // arrange
        await using var server = new FakeSmtpServer(advertiseAuth: false);
        var sut = CreateSut(server.Port);

        // act
        using var client = await sut.CreateAsync(TestContext.Current.CancellationToken);

        // assert
        Assert.True(client.IsConnected);
    }

    [Fact]
    public async Task CreateAsync_WithCredentialsAndAuthCapability_Authenticates() {
        // arrange
        await using var server = new FakeSmtpServer(advertiseAuth: true);
        var sut = CreateSut(server.Port, "user", "pass");

        // act
        using var client = await sut.CreateAsync(TestContext.Current.CancellationToken);

        // assert
        Assert.Multiple(
            () => Assert.True(client.IsAuthenticated),
            () => Assert.Contains(server.Commands, c => c.StartsWith("AUTH PLAIN", StringComparison.OrdinalIgnoreCase))
        );
    }

    [Fact]
    public async Task CreateAsync_WithCredentialsButNoAuthCapability_SkipsAuthentication() {
        // arrange
        await using var server = new FakeSmtpServer(advertiseAuth: false);
        var sut = CreateSut(server.Port, "user", "pass");

        // act
        using var client = await sut.CreateAsync(TestContext.Current.CancellationToken);

        // assert
        Assert.False(client.IsAuthenticated);
    }

    [Fact]
    public async Task CreateAsync_WithoutCredentials_SkipsAuthentication() {
        // arrange
        await using var server = new FakeSmtpServer(advertiseAuth: true);
        var sut = CreateSut(server.Port);

        // act
        using var client = await sut.CreateAsync(TestContext.Current.CancellationToken);

        // assert
        Assert.False(client.IsAuthenticated);
    }

    [Fact]
    public async Task CreateAsync_WhenServerUnreachable_Throws() {
        // arrange
        var sut = CreateSut(port: 1);

        // act & assert
        await Assert.ThrowsAnyAsync<Exception>(() => sut.CreateAsync(TestContext.Current.CancellationToken));
    }
}

[UnitTest]
public class MailkitRegistrationTests {
    [Theory]
    [InlineData("u", "p", true)]
    [InlineData("u", null, false)]
    [InlineData(null, "p", false)]
    [InlineData(" ", " ", false)]
    public void MailingOptions_UseCredentials_RequiresUsernameAndPassword(string? username, string? password, bool expected) {
        // arrange
        var sut = new MailingOptions { Username = username, Password = password };

        // act & assert
        Assert.Equal(expected, sut.UseCredentials);
    }

    [Fact]
    public void MailingOptions_HasExpectedDefaults() {
        // act
        var sut = new MailingOptions();

        // assert
        Assert.Multiple(
            () => Assert.Equal("localhost", sut.Host),
            () => Assert.Equal(25, sut.Port)
        );
    }

    [Fact]
    public void RegisterMailkitMailing_RegistersMailingAsSingleton() {
        // arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // act
        var returned = services.RegisterMailkitMailing();
        using var provider = services.BuildServiceProvider();

        // assert
        Assert.Multiple(
            () => Assert.Same(services, returned),
            () => Assert.IsType<MailkitMailing>(provider.GetRequiredService<IMailing>()),
            () => Assert.Same(provider.GetRequiredService<IMailing>(), provider.GetRequiredService<IMailing>())
        );
    }
}
