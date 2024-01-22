using System.Net.Security;
using System.Security.Authentication;
#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.ProducerConsumer.RabbitMQ.Options {
    public sealed class SslOptions {
        #region Public Static Read-Only Properties

        public static SslOptions Default => new();

        #endregion

        #region Public Properties

        public bool Enabled { get; set; }

        public string? ServerName { get; }

        public SslProtocols Protocol { get; set; }

        public SslPolicyErrors PolicyError { get; set; }

        #endregion

        #region Public Constructors

        public SslOptions() {
            ServerName = Environment.GetEnvironmentVariable(Root.EnvTokens.RABBITMQ_SSL_SERVERNAME);
        }

        #endregion

        #region Public Methods

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(returnValue: true, nameof(ServerName))]
#endif
        public bool IsAvailable()
            => Enabled &&
               !string.IsNullOrWhiteSpace(ServerName) &&
               Protocol != SslProtocols.None;

        #endregion
    }
}
