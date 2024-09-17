﻿#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.ProducerConsumer.RabbitMQ.Options;

public sealed record ServerOptions {
    public static ServerOptions Default => new();

    private SslOptions? _ssl;
    private CertificateOptions? _certificate;

    public string Protocol { get; set; } = "amqp";

    /// <summary>
    /// Gets or sets the username. Default value is <c>guest</c>.
    /// </summary>
    public string Username { get; set; } = Root.Defaults.RABBITMQ_USER;

    /// <summary>
    /// Gets or sets the user password. Default value is <c>guest</c>.
    /// </summary>
    public string Password { get; set; } = Root.Defaults.RABBITMQ_PASS;

    public string Hostname { get; set; } = "localhost";

    public int Port { get; set; } = 5672;

    public string VirtualHost { get; set; } = "/";

    public SslOptions Ssl {
        get => _ssl ??= SslOptions.Default;
        set => _ssl = value;
    }

    public CertificateOptions Certificate {
        get => _certificate ??= CertificateOptions.Default;
        set => _certificate = value;
    }

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(returnValue: true, nameof(Username), nameof(Password))]
#endif
    internal bool UseCredentials
        => !string.IsNullOrWhiteSpace(Username) &&
           !string.IsNullOrWhiteSpace(Password);

    internal string GetConnectionString()
        => UseCredentials
            ? $"{Protocol}://{Username}:{Password}@{Hostname}:{Port}{VirtualHost}"
            : $"{Protocol}://{Hostname}:{Port}{VirtualHost}";
}