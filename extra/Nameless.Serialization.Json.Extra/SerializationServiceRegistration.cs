using Autofac;
using Nameless.IoC.Autofac;

namespace Nameless.Serialization.Json {
    /// <summary>
    /// The Serialization service registration.
    /// </summary>
    public sealed class SerializationServiceRegistration : ServiceRegistrationBase {

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="ISerializer"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType SerializerLifetimeScope { get; set; } = LifetimeScopeType.Singleton;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<JsonSerializer> ()
                .As<ISerializer> ()
                .SetLifetimeScope (SerializerLifetimeScope);

            base.Load (builder);
        }

        #endregion Public Override Methods
    }
}