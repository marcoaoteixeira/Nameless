using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nameless.PubSub.RabbitMQ;

[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
public sealed record ServerSettings {
    private CertificateSettings? _certificate;
    private SslSettings? _ssl;

    private string DebuggerDisplayValue
        => UseCredentials
            ? $"{Protocol}://{Username}:{Password}@{Hostname}:{Port}{VirtualHost}"
            : $"{Protocol}://{Hostname}:{Port}{VirtualHost}";

    public string Protocol { get; set; } = "amqp";

    /// <summary>
    ///     Gets or sets the username. Default value is <c>guest</c>.
    /// </summary>
    public string Username { get; set; } = Internals.Defaults.RabbitMQUser;

    /// <summary>
    ///     Gets or sets the user password. Default value is <c>guest</c>.
    /// </summary>
    public string Password { get; set; } = Internals.Defaults.RabbitMQPassword;

    public string Hostname { get; set; } = "localhost";

    public int Port { get; set; } = 5672;

    public string VirtualHost { get; set; } = "/";

    public SslSettings Ssl {
        get => _ssl ??= new SslSettings();
        set => _ssl = value;
    }

    public CertificateSettings Certificate {
        get => _certificate ??= new CertificateSettings();
        set => _certificate = value;
    }

    [MemberNotNullWhen(true, nameof(Username), nameof(Password))]
    internal bool UseCredentials
        => !string.IsNullOrWhiteSpace(Username) &&
           !string.IsNullOrWhiteSpace(Password);
}