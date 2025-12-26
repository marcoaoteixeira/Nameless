using System.Diagnostics.CodeAnalysis;
using MailKit.Security;

namespace Nameless.Mailing.MailKit;

/// <summary>
///     The configuration for mailing client.
/// </summary>
public sealed class MailingOptions {
    /// <summary>
    ///     Whether to use credentials for SMTP authentication.
    /// </summary>
    [MemberNotNullWhen(returnValue: true, nameof(Username), nameof(Password))]
    internal bool UseCredentials
        => !string.IsNullOrWhiteSpace(Username) &&
           !string.IsNullOrWhiteSpace(Password);

    /// <summary>
    ///     Gets or sets the SMTP server address. Default value is "localhost".
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    ///     Gets or sets the SMTP server port. Default value is 25.
    /// </summary>
    public int Port { get; set; } = 25;

    /// <summary>
    ///     Gets or sets the username credential.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    ///     Gets or sets the password credential.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    ///     Gets or sets secure socket option to use if connection is SSL.
    /// </summary>
    public SecureSocketOptions SecureSocket { get; set; }
}