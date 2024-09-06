using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using Nameless.Mailing.MailKit.Options;

namespace Nameless.Mailing.MailKit.Impl;

/// <summary>
/// Implementation of <see cref="ISmtpClientFactory"/>
/// </summary>
public sealed class SmtpClientFactory : ISmtpClientFactory {
    private readonly MailServerOptions _options;

    /// <summary>
    /// Initializes a new instance of <see cref="SmtpClientFactory"/>
    /// </summary>
    /// <param name="options">The SMTP options.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="options"/> is <c>null</c>.
    /// </exception>
    public SmtpClientFactory(IOptions<MailServerOptions> options) {
        _options = Prevent.Argument.Null(options, nameof(options)).Value;
    }

    /// <inhertidoc />
    public async Task<ISmtpClient> CreateAsync(CancellationToken cancellationToken = default) {
        var client = new SmtpClient();

        await client.ConnectAsync(_options.Host,
                                  _options.Port,
                                  _options.SecureSocket,
                                  cancellationToken);

        return client.Capabilities.HasFlag(SmtpCapabilities.Authentication) && _options.UseCredentials
            ? await AuthenticateSmtpClient(client, _options, cancellationToken)
            : client;
    }

    private static async Task<ISmtpClient> AuthenticateSmtpClient(SmtpClient client, MailServerOptions mailServerOptions, CancellationToken cancellationToken) {
        client.AuthenticationMechanisms.Remove("XOAUTH2");

        await client.AuthenticateAsync(mailServerOptions.Username,
                                       mailServerOptions.Password,
                                       cancellationToken);

        return client;
    }
}