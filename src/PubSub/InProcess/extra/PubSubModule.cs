using Autofac;
using Nameless.DependencyInjection.Autofac;

namespace Nameless.PubSub {
    /// <summary>
    /// The PubSub service registration.
    /// </summary>
    public sealed class PubSubModule : ModuleBase {

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
    }
}