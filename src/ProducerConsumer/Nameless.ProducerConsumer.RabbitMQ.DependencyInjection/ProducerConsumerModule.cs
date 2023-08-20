using System.Reflection;
using Autofac;
using Autofac.Core.Registration;
using Microsoft.Extensions.Configuration;
using Nameless.Autofac;
using Nameless.ProducerConsumer.RabbitMQ;
using Nameless.ProducerConsumer.RabbitMQ.DependencyInjection;
using Nameless.ProducerConsumer.RabbitMQ.Services;
using Nameless.ProducerConsumer.RabbitMQ.Services.Impl;
using RabbitMQ.Client;

namespace Nameless.Data.SQLServer.DependencyInjection {
    public sealed class ProducerConsumerModule : ModuleBase {
        #region Private Constants

        private const string CONNECTION_MANAGER_TOKEN = $"{nameof(IConnectionManager)}::0ee463dd-f42d-407f-bc36-ecd2a5538faf";
        private const string CHANNEL_MANAGER_TOKEN = $"{nameof(IChannelManager)}::9985853c-25aa-4d0d-84af-581d80fac738";

        #endregion

        #region Public Constructors

        public ProducerConsumerModule()
            : base(Array.Empty<Assembly>()) { }

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ResolveConnectionManager)
                .Named<IConnectionManager>(CONNECTION_MANAGER_TOKEN)
                .SingleInstance();

            builder
                .Register(ResolveChannelManager)
                .Named<IChannelManager>(CHANNEL_MANAGER_TOKEN)
                .SingleInstance();

            builder
                .Register(ResolveChannel)
                .As<IModel>()
                .SingleInstance();

            builder
                .Register(ResolveBootstrapper)
                .As<IStartable>()
                .InstancePerDependency();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static RabbitMQOptions? GetRabbitMQOptions(IComponentContext ctx) {
            var configuration = ctx.ResolveOptional<IConfiguration>();
            var options = configuration?
                .GetSection(nameof(RabbitMQOptions).RemoveTail(new[] { "Options" }))
                .Get<RabbitMQOptions>();

            return options;
        }

        private static IConnectionManager ResolveConnectionManager(IComponentContext ctx) {
            var options = GetRabbitMQOptions(ctx);
            var result = new ConnectionManager(options);

            return result;
        }

        private static IChannelManager ResolveChannelManager(IComponentContext ctx) {
            var connectionManager = ctx.ResolveNamed<IConnectionManager>(CONNECTION_MANAGER_TOKEN);
            var connection = connectionManager.GetConnection();
            var result = new ChannelManager(connection);

            return result;
        }

        private static IModel ResolveChannel(IComponentContext ctx) {
            var channelManager = ctx.ResolveNamed<IChannelManager>(CHANNEL_MANAGER_TOKEN);
            var result = channelManager.GetChannel();

            return result;
        }

        private static IStartable ResolveBootstrapper(IComponentContext ctx) {
            var channel = ctx.Resolve<IModel>();
            var options = GetRabbitMQOptions(ctx);
            var result = new Bootstrapper(channel, options);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static IModuleRegistrar AddProducerConsumer(this ContainerBuilder self)
            => self.RegisterModule<ProducerConsumerModule>();

        #endregion
    }
}