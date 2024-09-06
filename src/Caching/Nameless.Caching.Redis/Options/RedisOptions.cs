namespace Nameless.Caching.Redis.Options;

/// <summary>
/// Provides properties to configure Redis.
/// </summary>
public sealed record RedisOptions {
    /// <summary>
    /// Gets a default options object.
    /// </summary>
    public static RedisOptions Default => new();

    private SslOptions? _ssl;
    private CertificateOptions? _certificate;

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

    /// <summary>
    /// Gets or sets SSL options.
    /// </summary>
    public SslOptions Ssl {
        get => _ssl ??= SslOptions.Default;
        set => _ssl = value;
    }

    /// <summary>
    /// Gets or sets certificate options.
    /// </summary>
    public CertificateOptions Certificate {
        get => _certificate ??= CertificateOptions.Default;
        set => _certificate = value;
    }
}