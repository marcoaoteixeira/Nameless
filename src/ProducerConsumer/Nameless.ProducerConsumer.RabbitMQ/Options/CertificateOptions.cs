#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.ProducerConsumer.RabbitMQ.Options {
    public sealed class CertificateOptions {
        #region Public Static Read-Only Properties

        public static CertificateOptions Default => new();

        #endregion

        #region Public Properties

        public string? Pfx { get; set; }

        public string? Pem { get; set; }

        public string? Password { get; set; }

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(returnValue: true, nameof(Pfx), nameof(Pem), nameof(Password))]
#endif
        public bool IsAvailable
            => !string.IsNullOrWhiteSpace(Pfx) &&
               !string.IsNullOrWhiteSpace(Pem) &&
               !string.IsNullOrWhiteSpace(Password);

        #endregion
    }
}
