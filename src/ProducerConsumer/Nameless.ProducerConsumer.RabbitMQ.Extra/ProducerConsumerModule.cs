using Autofac;
using Nameless.Autofac;
using Nameless.ProducerConsumer.RabbitMQ.Services;
using Nameless.ProducerConsumer.RabbitMQ.Services.Impl;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ {
    /// <summary>
    /// The PubSub service registration.
    /// </summary>
    public sealed class ProducerConsumerModule : ModuleBase {
        #region Private Constants

        private const string CONNECTION_MANAGER_TOKEN = "ConnectionManager.d78abe02-49ce-424d-8e7a-df2ea4de837e";
        private const string CHANNEL_MANAGER_TOKEN = "ChannelManager.e9aa434b-3418-451f-848e-a0999bb71ac2";
        private const string CHANNEL_TOKEN = "Channel.7e06234f-d326-4cba-afe2-24a2c2c288f1";
        private const string BOOTSTRAPPER_TOKEN = "Bootstrapper.f27aecdd-c79f-457c-b9f3-10a8629219af";

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
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
                .Named<IModel>(CHANNEL_TOKEN)
                .SingleInstance();

            builder
                .Register(ProducerServiceResolver)
                .As<IProducerService>()
                .SingleInstance();

            builder
                .Register(ConsumerServiceResolver)
                .As<IConsumerService>()
                .SingleInstance();

            builder
                .Register(BootstrapperResolver)
                .Named<Bootstrapper>(BOOTSTRAPPER_TOKEN)
                .AutoActivate();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IConnectionManager ConnectionManagerResolver(IComponentContext context) {
            var options = context.ResolveOptional<ProducerConsumerOptions>();
            var connectionManager = new ConnectionManager(options);
            
            return connectionManager;
        }

        private static IChannelManager ChannelManagerResolver(IComponentContext context) {
            var connectionManager = context.ResolveNamed<IConnectionManager>(CONNECTION_MANAGER_TOKEN);
            var channelManager = new ChannelManager(connectionManager);

            return channelManager;
        }

        private static IModel ChannelResolver(IComponentContext context) {
            var channelManager = context.ResolveNamed<IChannelManager>(CHANNEL_MANAGER_TOKEN);
            var channel = channelManager.GetChannel();

            return channel;
        }

        private static IProducerService ProducerServiceResolver(IComponentContext context) {
            var channelManager = context.ResolveNamed<IChannelManager>(CHANNEL_MANAGER_TOKEN);
            var channel = channelManager.GetChannel();
            var producerService = new ProducerService(channel);

            return producerService;
        }

        private static IConsumerService ConsumerServiceResolver(IComponentContext context) {
            var channelManager = context.ResolveNamed<IChannelManager>(CHANNEL_MANAGER_TOKEN);
            var channel = channelManager.GetChannel();
            var consumerService = new ConsumerService(channel);

            return consumerService;
        }

        private static Bootstrapper BootstrapperResolver(IComponentContext context) {
            var channelManager = context.ResolveNamed<IChannelManager>(CHANNEL_MANAGER_TOKEN);
            var channel = channelManager.GetChannel();
            var options = context.ResolveOptional<ProducerConsumerOptions>() ?? ProducerConsumerOptions.Default;

            return new Bootstrapper(channel, options);
        }

        #endregion
    }
}
