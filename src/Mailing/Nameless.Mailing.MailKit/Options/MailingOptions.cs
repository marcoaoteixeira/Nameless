#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

using MailKit.Security;

namespace Nameless.Mailing.MailKit.Options;

/// <summary>
/// The configuration for mailing client.
/// </summary>
public sealed record MailingOptions {
    /// <summary>
    /// Gets or sets the SMTP server address. Default value is "localhost".
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Gets or sets the SMTP server port. Default value is 25.
    /// </summary>
    public int Port { get; set; } = 25;

    /// <summary>
    /// Gets or sets the username credential.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password credential.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets secure socket option to use if connection is SSL.
    /// Default value is <c><see cref="SecureSocketOptions.None"/></c>.
    /// </summary>
    public SecureSocketOptions SecureSocket { get; set; } = SecureSocketOptions.None;

#if NET6_0_OR_GREATER
    [MemberNotNullWhen(returnValue: true, nameof(Username), nameof(Password))]
#endif
    public bool UseCredentials
        => !string.IsNullOrWhiteSpace(Username) &&
           !string.IsNullOrWhiteSpace(Password);
}