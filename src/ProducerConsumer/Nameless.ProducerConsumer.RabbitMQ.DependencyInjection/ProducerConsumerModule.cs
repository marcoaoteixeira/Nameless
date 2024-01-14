using Autofac;
using Autofac.Core;
using Nameless.Autofac;
using Nameless.ProducerConsumer.RabbitMQ.Services;
using Nameless.ProducerConsumer.RabbitMQ.Services.Impl;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.DependencyInjection {
    public sealed class ProducerConsumerModule : ModuleBase {
        #region Private Constants

        private const string CONNECTION_MANAGER_TOKEN = $"{nameof(IConnectionManager)}::0ee463dd-f42d-407f-bc36-ecd2a5538faf";
        private const string CHANNEL_MANAGER_TOKEN = $"{nameof(IChannelManager)}::9985853c-25aa-4d0d-84af-581d80fac738";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ConnectionManagerResolver)
                .Named<IConnectionManager>(CONNECTION_MANAGER_TOKEN)
                .SingleInstance();

            builder
                .Register(ChannelManagerResolver)
                .Named<IChannelManager>(CHANNEL_MANAGER_TOKEN)
                .SingleInstance();

            builder
                .Register(ChannelResolver)
                .As<IModel>()
                .OnActivated(RabbitMQStartUpRoutine) // StartUp should only occurs once.
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IConnectionManager ConnectionManagerResolver(IComponentContext ctx) {
            var options = GetOptionsFromContext<RabbitMQOptions>(ctx)
                ?? RabbitMQOptions.Default;
            var result = new ConnectionManager(options);

            return result;
        }

        private static IChannelManager ChannelManagerResolver(IComponentContext ctx) {
            var connectionManager = ctx.ResolveNamed<IConnectionManager>(CONNECTION_MANAGER_TOKEN);
            var connection = connectionManager.GetConnection();
            var result = new ChannelManager(connection);

            return result;
        }

        private static IModel ChannelResolver(IComponentContext ctx) {
            var channelManager = ctx.ResolveNamed<IChannelManager>(CHANNEL_MANAGER_TOKEN);
            var result = channelManager.GetChannel();

            return result;
        }

        private static void RabbitMQStartUpRoutine(IActivatedEventArgs<IModel> args) {
            var options = GetOptionsFromContext<RabbitMQOptions>(args.Context)
                ?? RabbitMQOptions.Default;

            foreach (var exchange in options.Exchanges) {
                ConfigureExchange(args.Instance, exchange);
                ConfigureQueues(args.Instance, exchange.Name, exchange.Queues);
            }
        }

        private static void ConfigureExchange(IModel channel, ExchangeOptions exchange)
            => channel.ExchangeDeclare(
                    exchange: exchange.Name,
                    type: exchange.Type.GetDescription(),
                    durable: exchange.Durable,
                    autoDelete: exchange.AutoDelete,
                    arguments: exchange.Arguments
                );

        private static void ConfigureQueues(IModel channel, string exchangeName, IEnumerable<QueueOptions> queues) {
            foreach (var queue in queues) {
                channel.QueueDeclare(
                    queue: queue.Name,
                    durable: queue.Durable,
                    exclusive: queue.Exclusive,
                    autoDelete: queue.AutoDelete,
                    arguments: queue.Arguments
                );

                channel.QueueBind(
                    queue: queue.Name,
                    exchange: exchangeName,
                    routingKey: queue.RoutingKey ?? queue.Name,
                    arguments: queue.Bindings
                );
            }
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static ContainerBuilder RegisterProducerConsumerModule(this ContainerBuilder self) {
            self.RegisterModule<ProducerConsumerModule>();

            return self;
        }

        #endregion
    }
}