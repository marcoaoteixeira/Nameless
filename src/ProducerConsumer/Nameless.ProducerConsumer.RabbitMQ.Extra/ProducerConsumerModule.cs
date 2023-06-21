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
        private const string BOOTSTRAPPER_KEY = "Bootstrapper.f27aecdd-c79f-457c-b9f3-10a8629219af";

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

            builder
                .Register(BootstrapperResolver)
                .Named<Bootstrapper>(BOOTSTRAPPER_KEY)
                .AutoActivate();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IConnection ConnectionResolver(IComponentContext context) {
            var options = context.ResolveOptional<ProducerConsumerOptions>() ?? new();
            var factory = new ConnectionFactory {
                HostName = options.Server.Hostname,
                Port = options.Server.Port,
                UserName = options.Server.Username,
                Password = options.Server.Password
            };

            if (options.Server.UseSsl) {
                factory.Ssl = new(
                    serverName: options.Server.ServerName,
                    certificatePath: options.Server.CertificatePath,
                    enabled: true
                );
            }

            var connection = factory.CreateConnection();

            return connection;
        }

        private static IModel ChannelResolver(IComponentContext context) {
            var connection = context.ResolveNamed<IConnection>(CONNECTION_KEY);
            var channel = connection.CreateModel();

            return channel;
        }

        private static Bootstrapper BootstrapperResolver(IComponentContext context) {
            var connection = context.ResolveNamed<IConnection>(CONNECTION_KEY);
            var opts = context.ResolveOptional<ProducerConsumerOptions>() ?? new();

            return new Bootstrapper(connection, opts);
        }

        #endregion
    }
}
