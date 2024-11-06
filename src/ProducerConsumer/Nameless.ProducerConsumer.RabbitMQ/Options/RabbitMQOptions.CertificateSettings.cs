#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.ProducerConsumer.RabbitMQ.Options;

public sealed class CertificateSettings {
    public string? Pfx { get; set; }

    public string? Pem { get; set; }

    public string? Password { get; set; }

#if NET6_0_OR_GREATER
    [MemberNotNullWhen(returnValue: true, nameof(Pfx), nameof(Pem), nameof(Password))]
#endif
    internal bool IsAvailable
        => !string.IsNullOrWhiteSpace(Pfx) &&
           !string.IsNullOrWhiteSpace(Pem) &&
           !string.IsNullOrWhiteSpace(Password);
}