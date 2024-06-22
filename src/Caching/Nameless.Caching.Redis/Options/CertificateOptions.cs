#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.Caching.Redis.Options {
    public sealed class CertificateOptions {
        #region Public Static Read-Only Properties

        public static CertificateOptions Default => new();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the .pfx file path.
        /// </summary>
        public string Pfx { get; set; } = string.Empty;

        /// <summary>
        /// Gets the .pem file path.
        /// </summary>
        public string Pem { get; set; } = string.Empty;

        /// <summary>
        /// Gets the certificate password. This property is provided by the environment
        /// variable <see cref="Root.EnvTokens.REDIS_CERT_PASS"/>.
        /// </summary>
        public string? Password { get; } = Environment.GetEnvironmentVariable(Root.EnvTokens.REDIS_CERT_PASS);

        #endregion

        #region Public Methods

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(returnValue: true, members: [nameof(Pfx), nameof(Pem), nameof(Password)])]
#endif
        public bool IsAvailable()
            => !string.IsNullOrWhiteSpace(Pfx) &&
               !string.IsNullOrWhiteSpace(Pem) &&
               !string.IsNullOrWhiteSpace(Password);

        #endregion
    }
}
