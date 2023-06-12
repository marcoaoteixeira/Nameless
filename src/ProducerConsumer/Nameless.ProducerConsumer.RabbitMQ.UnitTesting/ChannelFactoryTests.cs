using Moq;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.UnitTesting {

    public class ChannelFactoryTests {

        [Test]
        public void CreateChannel() {
            var exchanges = new[] {
                new ExchangeSettings {
                    Name = "Default.Exchange",
                    Type = ExchangeType.Direct,
                    Durable = true,
                    AutoDelete = false,
                    Arguments = new Dictionary<string, object>(),
                    Queues = new[] {
                        new QueueSettings {
                            Name = "Default.Queue",
                            Durable = true,
                            AutoDelete = false,
                            RoutingKey = "Default.Queue",
                            Arguments = new Dictionary<string, object>(),
                        }
                    }
                }
            };

            var connection = new Mock<IConnection>();
            var model = new Mock<IModel>();

            connection
                .Setup(_ => _.CreateModel())
                .Returns(model.Object);

            var exchange = exchanges[0];
            model
                .Setup(_ => _.ExchangeDeclare(
                    exchange.Name,
                    exchange.Type.GetDescription(),
                    exchange.Durable,
                    exchange.AutoDelete,
                    exchange.Arguments
                )).Verifiable();

            var queue = exchanges[0].Queues[0];
            model
                .Setup(_ => _.QueueDeclare(
                    queue.Name,
                    queue.Durable,
                    queue.Exclusive,
                    queue.Durable,
                    queue.Arguments
                )).Verifiable();

            model
                .Setup(_ => _.QueueBind(
                    queue.Name,
                    exchange.Name,
                    queue.RoutingKey,
                    new Dictionary<string, object>()
                )).Verifiable();

            var channelFactory = new ChannelFactory(connection.Object);

            var channel = channelFactory.Create(exchanges);

            Assert.Multiple(() => {
                Assert.That(channel, Is.Not.Null);
                Assert.That(() => {
                    model.Verify(_ => _.ExchangeDeclare(
                        exchange.Name,
                        exchange.Type.GetDescription(),
                        exchange.Durable,
                        exchange.AutoDelete,
                        exchange.Arguments
                    ));
                    model.Verify(_ => _.QueueDeclare(
                        queue.Name,
                        queue.Durable,
                        queue.Exclusive,
                        queue.AutoDelete,
                        queue.Arguments
                    ));
                    model.Verify(_ => _.QueueBind(
                        queue.Name,
                        exchange.Name,
                        queue.RoutingKey,
                        It.IsAny<Dictionary<string, object>>()
                    ));
                }, Throws.Nothing);
            });
        }
    }
}
