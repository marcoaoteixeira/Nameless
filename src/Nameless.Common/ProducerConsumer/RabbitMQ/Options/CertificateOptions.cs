using System.Diagnostics.CodeAnalysis;

namespace Nameless.ProducerConsumer.RabbitMQ.Options;

/// <summary>
///     Represents the settings for RabbitMQ certificate configuration.
/// </summary>
public record CertificateOptions {
    /// <summary>
    ///     Whether the certificate settings are available for use.
    /// </summary>
    [MemberNotNullWhen(returnValue: true, nameof(CertPath), nameof(CertPassword))]
    public bool IsAvailable
        => !string.IsNullOrWhiteSpace(CertPath) &&
           !string.IsNullOrWhiteSpace(CertPassword);

    /// <summary>
    ///     Gets or sets the path to the certificate file.
    /// </summary>
    public string? CertPath { get; init; }

    /// <summary>
    ///     Gets or sets the password for the certificate file.
    /// </summary>
    public string? CertPassword { get; init; }
}