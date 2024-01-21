#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.ProducerConsumer.RabbitMQ.Options {
    public sealed class CertificateOptions {
        #region Public Static Read-Only Properties

        public static CertificateOptions Default => new();

        #endregion

        #region Public Constructors

        public CertificateOptions() {
            Pfx = Environment.GetEnvironmentVariable(Root.EnvTokens.RABBITMQ_CERT_PFX);
            Pem = Environment.GetEnvironmentVariable(Root.EnvTokens.RABBITMQ_CERT_PEM);
            Pass = Environment.GetEnvironmentVariable(Root.EnvTokens.RABBITMQ_CERT_PASS);
        }

        #endregion

        #region Public Properties

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(returnValue: true, nameof(Pfx), nameof(Pem), nameof(Pass))]
#endif
        public bool IsAvailable()
            => !string.IsNullOrWhiteSpace(Pfx) &&
               !string.IsNullOrWhiteSpace(Pem) &&
               !string.IsNullOrWhiteSpace(Pass);

        public string? Pfx { get; }

        public string? Pem { get; }

        public string? Pass { get; }

        #endregion
    }
}
