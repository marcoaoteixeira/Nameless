using Autofac;
using Nameless.DependencyInjection.Autofac;

namespace Nameless.Mailing.MailKit {
    /// <summary>
    /// The Mailing service registration.
    /// </summary>
    public sealed class MailingModule : ModuleBase {

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IMailingService"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Transient"/>.</remarks>
        public LifetimeScopeType MailingServiceLifetimeScope { get; set; } = LifetimeScopeType.Transient;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<MailingService> ()
                .As<IMailingService> ()
                .SetLifetimeScope (MailingServiceLifetimeScope);

            base.Load (builder);
        }

        #endregion
    }
}