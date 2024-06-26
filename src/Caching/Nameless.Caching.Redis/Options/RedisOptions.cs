﻿namespace Nameless.Caching.Redis.Options {
    public sealed class RedisOptions {
        #region Public Static Read-Only Properties

        public static RedisOptions Default => new();

        #endregion

        #region Private Fields

        private SslOptions? _ssl;
        private CertificateOptions? _certificate;

        #endregion

        #region Public Properties

        public string Host { get; set; } = "localhost";

        public int Port { get; set; } = 6379;

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string? Password { get; set; }

        public int KeepAlive { get; set; } = -1;

        public SslOptions Ssl {
            get => _ssl ??= SslOptions.Default;
            set => _ssl = value;
        }

        public CertificateOptions Certificate {
            get => _certificate ??= CertificateOptions.Default;
            set => _certificate = value;
        }

        #endregion
    }
}
