namespace Nameless.Caching.Redis.Options;

/// <summary>
/// Provides properties to configure Redis.
/// </summary>
public sealed record RedisOptions {
    /// <summary>
    /// Gets or sets Redis host.
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Gets or sets Redis port.
    /// </summary>
    public int Port { get; set; } = 6379;

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets information whether it should keep connection alive.
    /// </summary>
    public int KeepAlive { get; set; } = -1;

    private SslSettings? _ssl;
    /// <summary>
    /// Gets or sets SSL options.
    /// </summary>
    public SslSettings Ssl {
        get => _ssl ??= new SslSettings();
        set => _ssl = value;
    }

    private CertificateSettings? _certificate;
    /// <summary>
    /// Gets or sets certificate options.
    /// </summary>
    public CertificateSettings Certificate {
        get => _certificate ??= new CertificateSettings();
        set => _certificate = value;
    }
}