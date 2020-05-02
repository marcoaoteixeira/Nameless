using Autofac;
using Nameless.IoC.Autofac;
using Nameless.Security.Cryptography;

namespace Nameless.Security {
    /// <summary>
    /// The Security service registration.
    /// </summary>
    public sealed class SecurityServiceRegistration : ServiceRegistrationBase {

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IChecksumProvider"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType ChecksumProviderLifetimeScope { get; set; } = LifetimeScopeType.Singleton;

        /// <summary>
        /// Gets or sets the <see cref="IPasswordGenerator"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType PasswordGeneratorLifetimeScope { get; set; } = LifetimeScopeType.Singleton;

        /// <summary>
        /// Gets or sets the <see cref="ICryptoProvider"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType CryptoProviderLifetimeScope { get; set; } = LifetimeScopeType.Transient;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<Md5ChecksumProvider> ()
                .As<IChecksumProvider> ()
                .SetLifetimeScope (ChecksumProviderLifetimeScope);

            builder
                .RegisterType<RandomPasswordGenerator> ()
                .As<IPasswordGenerator> ()
                .SetLifetimeScope (PasswordGeneratorLifetimeScope);

            builder
                .RegisterType<RijndaelCryptoProvider> ()
                .As<ICryptoProvider> ()
                .SetLifetimeScope (CryptoProviderLifetimeScope);

            base.Load (builder);
        }

        #endregion
    }
}