namespace Nameless.Caching.Redis.Options {
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
        /// Gets the user name. This property is provided by the environment
        /// variable <see cref="Root.EnvTokens.REDIS_USER"/>.
        /// </summary>
        public string? Username { get; }

        /// <summary>
        /// Gets the password. This property is provided by the environment
        /// variable <see cref="Root.EnvTokens.REDIS_PASS"/>.
        /// </summary>
        public string? Password { get; }

        public int KeepAlive { get; set; } = -1;

        public SslOptions Ssl {
            get => _ssl ??= SslOptions.Default;
            set => _ssl = value ?? SslOptions.Default;
        }

        public CertificateOptions Certificate {
            get => _certificate ??= CertificateOptions.Default;
            set => _certificate = value ?? CertificateOptions.Default;
        }

        #endregion

        #region Public Constructors

        public RedisOptions() {
            Username = Environment.GetEnvironmentVariable(Root.EnvTokens.REDIS_USER);
            Password = Environment.GetEnvironmentVariable(Root.EnvTokens.REDIS_PASS);
        }

        #endregion
    }
}
