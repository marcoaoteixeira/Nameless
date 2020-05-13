using Autofac;
using Nameless.DependencyInjection.Autofac;

namespace Nameless.Services {
    /// <summary>
    /// The Services service registration.
    /// </summary>
    public sealed class ServicesModule : ModuleBase {

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IDateTimeProvider"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType DateTimeProviderLifetimeScope { get; set; } = LifetimeScopeType.Singleton;

        /// <summary>
        /// Gets or sets the <see cref="IXmlSchemaValidator"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType XmlSchemaValidatorLifetimeScope { get; set; } = LifetimeScopeType.Singleton;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<DateTimeProvider> ()
                .As<IDateTimeProvider> ()
                .SetLifetimeScope (DateTimeProviderLifetimeScope);

            builder
                .RegisterType<XmlSchemaValidator> ()
                .As<IXmlSchemaValidator> ()
                .SetLifetimeScope (XmlSchemaValidatorLifetimeScope);

            base.Load (builder);
        }

        #endregion
    }
}