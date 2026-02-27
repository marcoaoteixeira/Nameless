using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nameless.ProducerConsumer.RabbitMQ.Options;

[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
public class ServerOptions {
    private string DebuggerDisplayValue
        => UseCredentials
            ? $"{Protocol}://{Username}:{Password}@{Hostname}:{Port}{VirtualHost}"
            : $"{Protocol}://{Hostname}:{Port}{VirtualHost}";

    /// <summary>
    ///     Whether to use credentials for the RabbitMQ connection.
    /// </summary>
    [MemberNotNullWhen(returnValue: true, nameof(Username), nameof(Password))]
    internal bool UseCredentials
        => !string.IsNullOrWhiteSpace(Username) &&
           !string.IsNullOrWhiteSpace(Password);

    /// <summary>
    ///     Gets or sets the protocol to use. Default value is <c>amqp</c>.
    /// </summary>
    public string Protocol { get; set; } = "amqp";

    /// <summary>
    ///     Gets or sets the username. Default value is <c>guest</c>.
    /// </summary>
    public string Username { get; set; } = "guest";

    /// <summary>
    ///     Gets or sets the user password. Default value is <c>guest</c>.
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    ///     Gets or sets the hostname. Default value is <c>localhost</c>.
    /// </summary>
    public string Hostname { get; set; } = "localhost";

    /// <summary>
    ///     Gets or sets the port. Default value is <c>5672</c>.
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    ///     Gets or sets the virtual host. Default value is <c>/</c>.
    /// </summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>
    ///     Gets or sets the SSL settings for the RabbitMQ connection.
    /// </summary>
    public SslOptions Ssl { get; set; } = new();

    /// <summary>
    ///     Gets or sets the certificate settings for the RabbitMQ connection.
    /// </summary>
    public CertificateOptions Certificate { get; set; } = new();
}