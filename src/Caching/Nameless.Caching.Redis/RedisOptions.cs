using System.Security.Authentication;

namespace Nameless.Caching.Redis {

    public sealed class RedisOptions {

        #region Public Static Read-Only Fields

        public static readonly RedisOptions Default = new();

        #endregion

        #region Public Properties

        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 6379;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int KeepAlive { get; set; } = -1;
        public SslOptions? Ssl { get; set; }
        public CertificateOptions? Certificate { get; set; }

        #endregion
    }

    public sealed class SslOptions {

        #region Public Properties

        /// <summary>
        /// Gets or sets whether if use SSL connection.
        /// </summary>
        public bool UseSsl { get; set; }
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
        #region Public Properties

        /// <summary>
        /// Gets or sets the .pfx file path.
        /// </summary>
        public string? Pfx { get; set; }
        /// <summary>
        /// Gets or sets the .pem file path.
        /// </summary>
        public string? Pem { get; set; }
        /// <summary>
        /// Gets or sets the certificate password.
        /// </summary>
        public string? Pass { get; set; }

        #endregion
    }
}
