using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Nameless.Attributes;

namespace Nameless.ProducerConsumer.RabbitMQ.Options;

[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
[ConfigurationSectionName("Server")]
public record ServerOptions {
    private string DebuggerDisplayValue
        => UseCredentials
            ? $"{Protocol}://{Username}:{Password}@{Hostname}:{Port}{VirtualHost}"
            : $"{Protocol}://{Hostname}:{Port}{VirtualHost}";

    /// <summary>
    ///     Whether to use credentials for the RabbitMQ connection.
    ///     
    /// </summary>
    [MemberNotNullWhen(returnValue: true, nameof(Username), nameof(Password))]
    public bool UseCredentials
        => !string.IsNullOrWhiteSpace(Username) &&
           !string.IsNullOrWhiteSpace(Password);

    /// <summary>
    ///     Gets or sets the protocol to use. Default value is <c>amqp</c>.
    /// </summary>
    public string Protocol { get; init; } = "amqp";

    /// <summary>
    ///     Gets or sets the username.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    ///     Gets or sets the user password.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    ///     Gets or sets the hostname. Default value is <c>localhost</c>.
    /// </summary>
    public string Hostname { get; init; } = "localhost";

    /// <summary>
    ///     Gets or sets the port. Default value is <c>5672</c>.
    /// </summary>
    public int Port { get; init; } = 5672;

    /// <summary>
    ///     Gets or sets the virtual host. Default value is <c>/</c>.
    /// </summary>
    public string VirtualHost { get; init; } = "/";

    /// <summary>
    ///     Gets or sets the SSL settings for the RabbitMQ connection.
    /// </summary>
    public SslOptions Ssl { get; init; } = new();

    /// <summary>
    ///     Gets or sets the certificate settings for the RabbitMQ connection.
    /// </summary>
    public CertificateOptions Certificate { get; init; } = new();
}