using Autofac;

namespace Nameless.ProducerConsumer.RabbitMQ.UnitTesting {
    [Ignore("This test suite needs RabbitMQ locally.")]
    public class ProducerTests {

        private IContainer _container;
        private string _exchangeName = $"{typeof(ProducerTests).FullName}::Exchange";
        private string _queueName = $"{typeof(ProducerTests).FullName}::Queue";

        [OneTimeSetUp]
        public void OneTimeSetUp() {
            _container = ProducerConsumerContainer.Create(builder => {
                var opts = new ProducerConsumerOptions {
                    Server = new() {
                        Port = 55250
                    },
                    Exchanges = new[] {
                        new ExchangeOptions {
                            Name = _exchangeName,
                            Queues = new[] {
                                new QueueOptions {
                                    Name = _queueName
                                }
                            }
                        },
                    }
                };

                builder.RegisterInstance(opts);
            });
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() {
            _container.Dispose();
        }

        [Test]
        public void Produce_Message_To_Queue() {
            // arrange
            var producer = _container.Resolve<IProducerService>();

            // act && assert
            Assert.DoesNotThrow(() => {
                producer.Produce(_queueName, new { text = "This is a test." }, new Parameter(Parameters.Producer.ExchangeName, _exchangeName));
            });
        }
    }
}
