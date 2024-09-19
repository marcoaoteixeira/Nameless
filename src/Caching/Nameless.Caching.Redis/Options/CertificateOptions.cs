#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.Caching.Redis.Options;

/// <summary>
/// Provides properties to configure Redis certificates.
/// </summary>
public sealed record CertificateOptions {
    /// <summary>
    /// Gets a default instance of <see cref="CertificateOptions"/>.
    /// </summary>
    public static CertificateOptions Default => new();

    /// <summary>
    /// Gets or sets the .pfx file path.
    /// </summary>
    public string Pfx { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the .pem file path.
    /// </summary>
    public string Pem { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the certificate password.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets whether certificate options is available.
    /// </summary>
#if NET6_0_OR_GREATER
    [MemberNotNullWhen(returnValue: true, nameof(Pfx), nameof(Pem), nameof(Password))]
#endif
    public bool IsAvailable
        => !string.IsNullOrWhiteSpace(Pfx) &&
           !string.IsNullOrWhiteSpace(Pem) &&
           !string.IsNullOrWhiteSpace(Password);
}