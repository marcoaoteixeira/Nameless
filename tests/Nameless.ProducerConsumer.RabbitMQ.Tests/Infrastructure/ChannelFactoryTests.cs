using Moq;
using Nameless.ProducerConsumer.RabbitMQ.Mockers;
using Nameless.Testing.Tools;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

public class ChannelFactoryTests {
    [Fact]
    public async Task WhenCreatingChannel_ThenReturnsNewChannel() {
        // arrange
        var connectionMocker = new ConnectionMocker().WithCreateChannelAsync();
        var connectionManagerMocker = new ConnectionManagerMocker().WithGetConnectionAsync(connectionMocker.Build());
        var sut = CreateSut(connectionManagerMocker.Build());

        // act
        var actual = await sut.CreateAsync(CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(actual);

            connectionManagerMocker.Verify(mock => mock.GetConnectionAsync(It.IsAny<CancellationToken>()));
            connectionMocker.Verify(mock =>
                mock.CreateChannelAsync(It.IsAny<CreateChannelOptions>(), It.IsAny<CancellationToken>()));
        });
    }

    private static ChannelFactory CreateSut(IConnectionManager connectionManager = null) {
        connectionManager ??= Quick.Mock<IConnectionManager>();

        return new ChannelFactory(connectionManager);
    }
}