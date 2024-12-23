#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.PubSub.RabbitMQ.Options;

public sealed class CertificateSettings {
    public string? CertPath { get; set; }

    public string? CertPassword { get; set; }

#if NET8_0_OR_GREATER
    [MemberNotNullWhen(returnValue: true, nameof(CertPath), nameof(CertPassword))]
#endif
    internal bool IsAvailable
        => !string.IsNullOrWhiteSpace(CertPath) &&
           !string.IsNullOrWhiteSpace(CertPassword);
}