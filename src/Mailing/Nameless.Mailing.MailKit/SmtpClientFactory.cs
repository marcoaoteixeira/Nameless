﻿using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using Nameless.Mailing.MailKit.Options;

namespace Nameless.Mailing.MailKit;

/// <summary>
/// Implementation of <see cref="ISmtpClientFactory"/>
/// </summary>
public sealed class SmtpClientFactory : ISmtpClientFactory {
    private readonly IOptions<MailingOptions> _options;

    /// <summary>
    /// Initializes a new instance of <see cref="SmtpClientFactory"/>
    /// </summary>
    /// <param name="options">The SMTP options.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="options"/> is <c>null</c>.
    /// </exception>
    public SmtpClientFactory(IOptions<MailingOptions> options) {
        _options = Prevent.Argument.Null(options);
    }

    /// <inhertidoc />
    public async Task<ISmtpClient> CreateAsync(CancellationToken cancellationToken = default) {
        var client = new SmtpClient();

        await client.ConnectAsync(_options.Value.Host,
                                  _options.Value.Port,
                                  _options.Value.SecureSocket,
                                  cancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false);

        return client.Capabilities.HasFlag(SmtpCapabilities.Authentication) && _options.Value.UseCredentials
            ? await AuthenticateSmtpClient(client, _options.Value, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)
            : client;
    }

    private static async Task<ISmtpClient> AuthenticateSmtpClient(SmtpClient client, MailingOptions mailServerOptions, CancellationToken cancellationToken) {
        client.AuthenticationMechanisms.Remove("XOAUTH2");

        await client.AuthenticateAsync(mailServerOptions.Username,
                                       mailServerOptions.Password,
                                       cancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false);

        return client;
    }
}