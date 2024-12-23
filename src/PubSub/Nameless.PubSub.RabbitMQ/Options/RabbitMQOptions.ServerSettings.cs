using System.Diagnostics;
#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.PubSub.RabbitMQ.Options;

[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
public sealed record ServerSettings {
    private SslSettings? _ssl;
    private CertificateSettings? _certificate;

    private string DebuggerDisplayValue
        => UseCredentials
            ? $"{Protocol}://{Username}:{Password}@{Hostname}:{Port}{VirtualHost}"
            : $"{Protocol}://{Hostname}:{Port}{VirtualHost}";

    public string Protocol { get; set; } = "amqp";

    /// <summary>
    /// Gets or sets the username. Default value is <c>guest</c>.
    /// </summary>
    public string Username { get; set; } = Defaults.RABBITMQ_USER;

    /// <summary>
    /// Gets or sets the user password. Default value is <c>guest</c>.
    /// </summary>
    public string Password { get; set; } = Defaults.RABBITMQ_PASS;

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

#if NET8_0_OR_GREATER
        [MemberNotNullWhen(returnValue: true, nameof(Username), nameof(Password))]
#endif
    internal bool UseCredentials
        => !string.IsNullOrWhiteSpace(Username) &&
           !string.IsNullOrWhiteSpace(Password);

    
}