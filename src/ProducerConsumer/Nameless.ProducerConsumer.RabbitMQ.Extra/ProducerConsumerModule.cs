using Autofac;
using Autofac.Core;
using Nameless.Autofac;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ {

    /// <summary>
    /// The PubSub service registration.
    /// </summary>
    public sealed class ProducerConsumerModule : ModuleBase {

        #region Private Constants

        private const string CONNECTION_KEY = "Connection.d78abe02-49ce-424d-8e7a-df2ea4de837e";
        private const string CHANNEL_KEY = "Channel.e9aa434b-3418-451f-848e-a0999bb71ac2";

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ConnectionResolver)
                .Named<IConnection>(CONNECTION_KEY)
                .SingleInstance();

            builder
                .Register(ChannelResolver)
                .Named<IModel>(CHANNEL_KEY)
                .SingleInstance();

            builder
                .RegisterType<Producer>()
                .As<IProducer>()
                .WithParameter(ResolvedParameter.ForNamed<IModel>(CHANNEL_KEY))
                .SingleInstance();

            builder
                .RegisterType<Consumer>()
                .As<IConsumer>()
                .WithParameter(ResolvedParameter.ForNamed<IModel>(CHANNEL_KEY))
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IConnection ConnectionResolver(IComponentContext context) {
            var settings = context.ResolveOptional<RabbitMQSettings>() ?? new();
            var factory = new ConnectionFactory {
                HostName = settings.Server.Hostname,
                Port = settings.Server.Port,
                UserName = settings.Server.Username,
                Password = settings.Server.Password
            };

            if (settings.Server.UseSsl) {
                factory.Ssl = new(
                    serverName: settings.Server.ServerName,
                    certificatePath: settings.Server.CertificatePath,
                    enabled: true
                );
            }

            var connection = factory.CreateConnection();

            return connection;
        }

        private static IModel ChannelResolver(IComponentContext context) {
            var settings = context.ResolveOptional<RabbitMQSettings>() ?? new();
            var connection = context.ResolveNamed<IConnection>(CONNECTION_KEY);
            var factory = new ChannelFactory(connection);

            var channel = factory.Create(settings.Exchanges);

            return channel;
        }

        #endregion
    }
}
