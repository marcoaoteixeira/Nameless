#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.ProducerConsumer.RabbitMQ.Options {
    public sealed class CertificateOptions {
        #region Public Static Read-Only Properties

        public static CertificateOptions Default => new();

        #endregion

        #region Public Properties

        public string Pfx { get; set; } = string.Empty;

        public string Pem { get; set; } = string.Empty;

        public string Password { get; } = Environment.GetEnvironmentVariable(Root.EnvTokens.RABBITMQ_CERT_PASS)
                                       ?? string.Empty;

        #endregion

        #region Public Methods

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(returnValue: true, nameof(Pfx), nameof(Pem), nameof(Password))]
#endif
        public bool IsAvailable()
            => !string.IsNullOrWhiteSpace(Pfx) &&
               !string.IsNullOrWhiteSpace(Pem) &&
               !string.IsNullOrWhiteSpace(Password);

        #endregion
    }
}
