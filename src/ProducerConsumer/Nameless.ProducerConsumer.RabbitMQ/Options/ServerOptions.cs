#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.ProducerConsumer.RabbitMQ.Options {
    public sealed record ServerOptions {
        #region Public Static Read-Only Properties

        public static ServerOptions Default => new();

        #endregion

        #region Private Fields

        private SslOptions? _ssl;
        private CertificateOptions? _certificate;

        #endregion

        #region Public Properties

        public string Protocol { get; set; } = "amqp";

        /// <summary>
        /// Gets or sets the username. Default value is <c>guest</c>.
        /// </summary>
        public string Username { get; set; } = Root.Defaults.RABBITMQ_USER;

        /// <summary>
        /// Gets or sets the user password. Default value is <c>guest</c>.
        /// </summary>
        public string Password { get; set; } = Root.Defaults.RABBITMQ_PASS;

        public string Hostname { get; set; } = "localhost";

        public int Port { get; set; } = 5672;

        public string VirtualHost { get; set; } = "/";

        public SslOptions Ssl {
            get => _ssl ??= SslOptions.Default;
            set => _ssl = value;
        }

        public CertificateOptions Certificate {
            get => _certificate ??= CertificateOptions.Default;
            set => _certificate = value;
        }

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(returnValue: true, nameof(Username), nameof(Password))]
#endif
        public bool UseCredentials
            => !string.IsNullOrWhiteSpace(Username) &&
               !string.IsNullOrWhiteSpace(Password);

        #endregion

        #region Public Methods

        public string GetConnectionString()
            => UseCredentials
                ? $"{Protocol}://{Username}:{Password}@{Hostname}:{Port}{VirtualHost}"
                : $"{Protocol}://{Hostname}:{Port}{VirtualHost}";

        #endregion
    }
}
