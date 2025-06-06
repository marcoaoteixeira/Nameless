using Moq;
using Nameless.Testing.Tools.Mockers;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Mockers;

public sealed class ConnectionMocker : MockerBase<IConnection> {
    public ConnectionMocker WithChannel(IChannel channel) {
        MockInstance.Setup(mock => mock.CreateChannelAsync(It.IsAny<CreateChannelOptions>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(channel);

        return this;
    }
}