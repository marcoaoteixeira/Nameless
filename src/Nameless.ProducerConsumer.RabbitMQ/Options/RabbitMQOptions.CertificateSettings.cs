using System.Diagnostics.CodeAnalysis;

namespace Nameless.ProducerConsumer.RabbitMQ.Options;

/// <summary>
///     Represents the settings for RabbitMQ certificate configuration.
/// </summary>
public sealed class CertificateSettings {
    /// <summary>
    ///     Whether the certificate settings are available for use.
    /// </summary>
    [MemberNotNullWhen(true, nameof(CertPath), nameof(CertPassword))]
    internal bool IsAvailable
        => !string.IsNullOrWhiteSpace(CertPath) &&
           !string.IsNullOrWhiteSpace(CertPassword);

    /// <summary>
    ///     Gets or sets the path to the certificate file.
    /// </summary>
    public string? CertPath { get; set; }

    /// <summary>
    ///     Gets or sets the password for the certificate file.
    /// </summary>
    public string? CertPassword { get; set; }
}