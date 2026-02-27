using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using Nameless.Testing.Tools;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Helpers;
using Nameless.Testing.Tools.Mockers.Logging;
using RabbitMQ.Client.Exceptions;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

[IntegrationTest]
[Collection(nameof(RabbitContainerCollection))]
public class ConnectionManagerTests {
    [Fact]
    public async Task WhenGettingConnection_WhenRabbitMQInstanceIsRunning_ThenReturnsConnection() {
        // arrange
        var options = OptionsHelper.Create<RabbitMQOptions>(opts => {
            opts.Server = new ServerOptions { Port = RabbitContainer.HOST_PORT };
        });
        var sut = new ConnectionManager(options, Quick.Mock<ILogger<ConnectionManager>>());

        // act
        var actual = await sut.GetConnectionAsync(CancellationToken.None);

        // assert
        Assert.NotNull(actual);

        await sut.DisposeAsync();
    }

    [Fact]
    public async Task WhenGettingConnection_WhenServerNotFound_ThenThrowsBrokerUnreachableException() {
        // arrange
        var errorMessage = string.Empty;
        var loggerMocker = new LoggerMocker<ConnectionManager>().WithAnyLogLevel()
                                                                .WithLog(LogLevel.Error,
                                                                    message => errorMessage = message);
        var options = OptionsHelper.Create<RabbitMQOptions>(opts => {
            opts.Server = new ServerOptions { Port = 5000 };
        });
        var sut = new ConnectionManager(options, loggerMocker.Build());

        // act
        var exception = await Record.ExceptionAsync(() => sut.GetConnectionAsync(CancellationToken.None));

        // assert
        Assert.Multiple(() => {
            Assert.NotEmpty(errorMessage);
            Assert.IsType<BrokerUnreachableException>(exception);

            loggerMocker.VerifyError(message => message.Contains(value: "Unable to connect to broker"));
        });

        await sut.DisposeAsync();
    }
}