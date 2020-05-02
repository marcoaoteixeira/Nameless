using Autofac;
using Autofac.Core;
using Nameless.IoC.Autofac;
using RabbitMQ.Client;

namespace Nameless.PubSub.RabbitMQ {
    /// <summary>
    /// The PubSub service registration.
    /// </summary>
    public sealed class PubSubServiceRegistration : ServiceRegistrationBase {

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IPublisher"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType PublisherLifetimeScope { get; set; } = LifetimeScopeType.Singleton;

        /// <summary>
        /// Gets or sets the <see cref="ISubscriber"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType SubscriberLifetimeScope { get; set; } = LifetimeScopeType.Singleton;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<ConnectionFactory> ()
                .As<IConnectionFactory> ()
                .OnActivated (OnActivatedConnectionFactory)
                .SetLifetimeScope (LifetimeScopeType.Singleton);

            builder
                .RegisterType<Publisher> ()
                .As<IPublisher> ()
                .SetLifetimeScope (PublisherLifetimeScope);

            builder
                .RegisterType<Subscriber> ()
                .As<ISubscriber> ()
                .SetLifetimeScope (SubscriberLifetimeScope);

            base.Load (builder);
        }

        #endregion

        #region Private Static Methods

        private static void OnActivatedConnectionFactory (IActivatedEventArgs<ConnectionFactory> args) {
            var settings = args.Context.ResolveOptional<PubSubSettings> () ?? new PubSubSettings ();
            var factory = args.Instance;

            factory.HostName = settings.Server.Hostname;
            if (settings.Credentials != null) {
                factory.UserName = settings.Credentials.Username;
                factory.Password = settings.Credentials.Password;
            }
        }

        #endregion
    }
}