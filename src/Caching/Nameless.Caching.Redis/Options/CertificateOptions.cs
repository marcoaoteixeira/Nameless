#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.Caching.Redis.Options {
    public sealed class CertificateOptions {
        #region Public Static Read-Only Properties

        public static CertificateOptions Default => new();

        #endregion

        #region Public Constructors

        public CertificateOptions() {
            Pfx = Environment.GetEnvironmentVariable(Root.EnvTokens.REDIS_CERT_PFX);
            Pem = Environment.GetEnvironmentVariable(Root.EnvTokens.REDIS_CERT_PEM);
            Pass = Environment.GetEnvironmentVariable(Root.EnvTokens.REDIS_CERT_PASS);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the .pfx file path. This property is provided by the environment
        /// variable <see cref="Root.EnvTokens.REDIS_CERT_PFX"/>.
        /// </summary>
        public string? Pfx { get; }

        /// <summary>
        /// Gets the .pem file path. This property is provided by the environment
        /// variable <see cref="Root.EnvTokens.REDIS_CERT_PEM"/>.
        /// </summary>
        public string? Pem { get; }

        /// <summary>
        /// Gets the certificate password. This property is provided by the environment
        /// variable <see cref="Root.EnvTokens.REDIS_CERT_PASS"/>.
        /// </summary>
        public string? Pass { get; }

        #endregion

        #region Public Methods

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(true, nameof(Pfx), nameof(Pem), nameof(Pass))]
#endif
        public bool IsAvailable()
            => !string.IsNullOrWhiteSpace(Pfx) &&
               !string.IsNullOrWhiteSpace(Pem) &&
               !string.IsNullOrWhiteSpace(Pass);

        #endregion
    }
}
