using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using System.Security.Authentication;

namespace Nameless.ProducerConsumer.RabbitMQ.Options;

/// <summary>
///     Represents the SSL settings for RabbitMQ connections.
/// </summary>
public sealed class SslSettings {
    /// <summary>
    ///     Whether SSL is enabled for RabbitMQ connections.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    ///     Gets or sets the server name for SSL connections.
    /// </summary>
    public string ServerName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the SSL protocol to use for RabbitMQ connections.
    /// </summary>
    public SslProtocols Protocol { get; set; }

    /// <summary>
    ///     Gets or sets the acceptable SSL policy errors for RabbitMQ connections.
    /// </summary>
    public SslPolicyErrors AcceptablePolicyErrors { get; set; }

    /// <summary>
    ///     Whether the SSL settings are available for use.
    /// </summary>
    [MemberNotNullWhen(returnValue: true, [nameof(ServerName)])]
    public bool IsAvailable
        => Enabled &&
           !string.IsNullOrWhiteSpace(ServerName) &&
           Protocol != SslProtocols.None;
}