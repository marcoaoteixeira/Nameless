using Nameless.ProducerConsumer.RabbitMQ.Mockers;
using Nameless.Testing.Tools;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

public class ChannelFactoryTests {
    [Fact]
    public async Task WhenCreatingChannel_WithExchangeName_ThenReturnsAChannel() {
        // arrange
        const string ExchangeName = "Exchange_b0b8fb97_c6d8_44f6_898a_af74b73d419a";
        var channel = Fast.Mock<IChannel>();
        var connection = new ConnectionMocker().WithChannel(channel)
                                               .Build();
        var sut = CreateSut(connection: connection);

        // act
        var actual = await sut.CreateAsync(ExchangeName, CancellationToken.None);

        // assert
        Assert.NotNull(actual);
    }

    private static IChannelFactory CreateSut(IConnectionManager connectionManager = null, IConnection connection = null, IChannelConfigurator channelConfigurator = null) {
        connection ??= new ConnectionMocker().Build();
        connectionManager ??= new ConnectionManagerMocker().WithGetConnectionAsync(connection)
                                                           .Build();
        channelConfigurator ??= new ChannelConfiguratorMocker().Build();

        return new ChannelFactory(connectionManager, channelConfigurator);
    }
}
