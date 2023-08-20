using System.Diagnostics.CodeAnalysis;
using System.Security.Authentication;

namespace Nameless.Caching.Redis {
    public sealed class RedisOptions {
        #region Public Static Read-Only Properties

        public static RedisOptions Default => new();

        #endregion

        #region Public Constructors

        public RedisOptions() {
            Username = Environment.GetEnvironmentVariable(Root.EnvTokens.REDIS_USER)
                ?? Root.Defaults.REDIS_USER;
            Password = Environment.GetEnvironmentVariable(Root.EnvTokens.REDIS_PASS)
                ?? Root.Defaults.REDIS_PASS;
        }

        #endregion

        #region Public Properties

        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 6379;
        /// <summary>
        /// Gets the username. Use the environment variable REDIS_USER.
        /// </summary>
        public string? Username { get; }
        /// <summary>
        /// Gets the password. Use the environment variable REDIS_PASS.
        /// </summary>
        public string? Password { get; }
        public int KeepAlive { get; set; } = -1;
        public SslOptions Ssl { get; set; } = new();
        public CertificateOptions Certificate { get; set; } = new();

        #endregion
    }

    public sealed class SslOptions {

        #region Public Properties

        [MemberNotNullWhen(true, nameof(Host))]
        public bool Available
            => !string.IsNullOrWhiteSpace(Host) &&
               Port > 0 &&
               SslProtocol != SslProtocols.None;

        /// <summary>
        /// Gets or sets the SSL host.
        /// </summary>
        public string? Host { get; set; }
        /// <summary>
        /// Gets or sets the SSL port.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Gets or sets the SSL protocol.
        /// </summary>
        public SslProtocols SslProtocol { get; set; }

        #endregion
    }

    public sealed class CertificateOptions {
        #region Public Constructors

        public CertificateOptions() {
            Pfx = Environment.GetEnvironmentVariable(Root.EnvTokens.REDIS_CERT_PFX);
            Pem = Environment.GetEnvironmentVariable(Root.EnvTokens.REDIS_CERT_PEM);
            Pass = Environment.GetEnvironmentVariable(Root.EnvTokens.REDIS_CERT_PASS);
        }

        #endregion

        #region Public Properties

        [MemberNotNullWhen(true, nameof(Pfx), nameof(Pem), nameof(Pass))]
        public bool Available
            => !string.IsNullOrWhiteSpace(Pfx) &&
               !string.IsNullOrWhiteSpace(Pem) &&
               !string.IsNullOrWhiteSpace(Pass);

        /// <summary>
        /// Gets the .pfx file path. Use the environment variable REDIS_CERT_PFX.
        /// </summary>
        public string? Pfx { get; }
        /// <summary>
        /// Gets the .pem file path. Use the environment variable REDIS_CERT_PEM.
        /// </summary>
        public string? Pem { get; }
        /// <summary>
        /// Gets the certificate password. Use the environment variable REDIS_CERT_PASS.
        /// </summary>
        public string? Pass { get; }

        #endregion
    }
}
