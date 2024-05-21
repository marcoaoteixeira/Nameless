using Autofac;
using Autofac.Core;
using Nameless.Autofac;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using Nameless.ProducerConsumer.RabbitMQ.Services;
using Nameless.ProducerConsumer.RabbitMQ.Services.Impl;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.DependencyInjection {
    public sealed class ProducerConsumerModule : ModuleBase {
        #region Private Constants

        private const string CHANNEL_FACTORY_TOKEN = $"{nameof(IChannelFactory)}::9985853c-25aa-4d0d-84af-581d80fac738";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ChannelFactoryResolver)
                .Named<IChannelFactory>(CHANNEL_FACTORY_TOKEN)
                .SingleInstance();

            builder
                .Register(ChannelResolver)
                .As<IModel>()
                // Here, our Channel will be a singleton.
                // So, this will be the perfect place to setup
                // our StartUp code. This must occurs only once.
                .OnActivated(StartUp)
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static ChannelFactory ChannelFactoryResolver(IComponentContext ctx)
            => new (options: ctx.GetPocoOptions<RabbitMQOptions>(),
                    logger: ctx.GetLogger<ChannelFactory>());

        private static IModel ChannelResolver(IComponentContext ctx) {
            var channelFactory = ctx
                .ResolveNamed<IChannelFactory>(CHANNEL_FACTORY_TOKEN);
            var result = channelFactory.CreateChannel();

            return result;
        }

        private static void StartUp(IActivatedEventArgs<IModel> args) {
            var options = args.Context.GetPocoOptions<RabbitMQOptions>();
            var channel = args.Instance;

            // when we declare a exchange/queue, if the exchange/queue
            // doesn't exist, it will be created for us. Otherwise,
            // RabbitMQ will just ignore.

            foreach (var exchange in options.Exchanges) {
                // let's declare our exchange
                DeclareExchange(channel, exchange);

                foreach (var queue in exchange.Queues) {
                    // let's declare our queue
                    DeclareQueue(channel, queue);

                    // let's declare our bindings
                    foreach (var binding in queue.Bindings) {
                        DeclareBinding(channel, exchange, queue, binding);
                    }
                }
            }
        }

        private static void DeclareBinding(IModel channel, ExchangeOptions exchange, QueueOptions queue, BindingOptions binding)
            => channel.QueueBind(queue: queue.Name,
                                 exchange: exchange.Name,
                                 routingKey: binding.RoutingKey,
                                 arguments: binding.Arguments);

        private static void DeclareQueue(IModel channel, QueueOptions queue)
            => channel.QueueDeclare(queue: queue.Name,
                                    durable: queue.Durable,
                                    exclusive: queue.Exclusive,
                                    autoDelete: queue.AutoDelete,
                                    arguments: queue.Arguments);

        private static void DeclareExchange(IModel channel, ExchangeOptions exchange)
            => channel.ExchangeDeclare(exchange: exchange.Name,
                                       type: exchange.Type.GetDescription(),
                                       durable: exchange.Durable,
                                       autoDelete: exchange.AutoDelete,
                                       arguments: exchange.Arguments);
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