using Autofac;
using Nameless.DependencyInjection.Autofac;

namespace Nameless.Notification {
    /// <summary>
    /// The Notification service registration.
    /// </summary>
    public sealed class NotificationModule : ModuleBase {

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="INotifier"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Transient"/>.</remarks>
        public LifetimeScopeType NotifierLifetimeScope { get; set; } = LifetimeScopeType.Transient;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<Notifier> ()
                .As<INotifier> ()
                .SetLifetimeScope (NotifierLifetimeScope);

            base.Load (builder);
        }

        #endregion
    }
}