using System.Diagnostics.CodeAnalysis;

namespace Nameless.PubSub.RabbitMQ;

public sealed class CertificateSettings {
    public string? CertPath { get; set; }

    public string? CertPassword { get; set; }

    [MemberNotNullWhen(true, nameof(CertPath), nameof(CertPassword))]
    internal bool IsAvailable
        => !string.IsNullOrWhiteSpace(CertPath) &&
           !string.IsNullOrWhiteSpace(CertPassword);
}