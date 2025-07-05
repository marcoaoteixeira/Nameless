using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using Nameless.Testing.Tools;
using Nameless.Testing.Tools.Mockers;
using RabbitMQ.Client.Exceptions;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

[Collection(nameof(RabbitContainerCollection))]
public class ConnectionManagerTests {
    [Fact]
    public async Task WhenGettingConnection_WhenRabbitMQInstanceIsRunning_ThenReturnsConnection() {
        // arrange
        var options = new OptionsMocker<RabbitMQOptions>().WithValue(new RabbitMQOptions {
            Server = new ServerSettings {
                Port = RabbitContainer.HOST_PORT
            }
        });
        var sut = new ConnectionManager(options.Build(), Quick.Mock<ILogger<ConnectionManager>>());

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
                                                                .WithLogCallback(LogLevel.Error, message => errorMessage = message);
        var options = new OptionsMocker<RabbitMQOptions>().WithValue(new RabbitMQOptions {
            Server = new ServerSettings {
                Port = 5000
            }
        });
        var sut = new ConnectionManager(options.Build(), loggerMocker.Build());

        // act
        var exception = await Record.ExceptionAsync(() => sut.GetConnectionAsync(CancellationToken.None));

        // assert
        Assert.Multiple(() => {
            Assert.NotEmpty(errorMessage);
            Assert.IsType<BrokerUnreachableException>(exception);

            loggerMocker.VerifyErrorCall(message => message.Contains("Unable to connect to broker"));
        });

        await sut.DisposeAsync();
    }
}
