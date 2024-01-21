#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.ProducerConsumer.RabbitMQ.Options {
    public sealed class ServerOptions {
        #region Public Static Read-Only Properties

        public static ServerOptions Default => new();

        #endregion

        #region Private Fields

        private SslOptions? _ssl;
        private CertificateOptions? _certificate;

        #endregion

        #region Public Constructors

        public ServerOptions() {
            Username = Environment.GetEnvironmentVariable(Root.EnvTokens.RABBITMQ_USER)
                ?? Root.Defaults.RABBITMQ_USER;
            Password = Environment.GetEnvironmentVariable(Root.EnvTokens.RABBITMQ_PASS)
                ?? Root.Defaults.RABBITMQ_PASS;
        }

        #endregion

        #region Public Properties

        public string Protocol { get; set; } = "amqp";

        /// <summary>
        /// Gets the user name. This property is provided by the environment
        /// variable <see cref="Root.EnvTokens.RABBITMQ_USER"/>. The default
        /// value is <c>guest</c>.
        /// </summary>
        public string Username { get; }
        /// <summary>
        /// Gets the user password. This property is provided by the environment
        /// variable <see cref="Root.EnvTokens.RABBITMQ_PASS"/>. The default
        /// value is <c>guest</c>.
        /// </summary>
        public string Password { get; }

        public string Hostname { get; set; } = "localhost";

        public int Port { get; set; } = 5672;

        public string VirtualHost { get; set; } = "/";

        public SslOptions Ssl {
            get => _ssl ??= SslOptions.Default;
            set => _ssl = value ?? SslOptions.Default;
        }

        public CertificateOptions Certificate {
            get => _certificate ??= CertificateOptions.Default;
            set => _certificate = value ?? CertificateOptions.Default;
        }

        #endregion

        #region Public Methods

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(true, nameof(Username), nameof(Password))]
#endif
        public bool UseCredentials()
            => !string.IsNullOrWhiteSpace(Username) &&
               !string.IsNullOrWhiteSpace(Password);

        #endregion

        #region Public Override Methods

        public override string ToString()
            => UseCredentials()
                ? $"{Protocol}://{Username}:{Password}@{Hostname}:{Port}{VirtualHost}"
                : $"{Protocol}://{Hostname}:{Port}{VirtualHost}";

        #endregion
    }
}
