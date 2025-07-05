using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;

namespace Nameless.Mailing.MailKit;

/// <summary>
///     Implementation of <see cref="ISmtpClientFactory" />
/// </summary>
public sealed class SmtpClientFactory : ISmtpClientFactory {
    private readonly IOptions<MailingOptions> _options;

    /// <summary>
    ///     Initializes a new instance of <see cref="SmtpClientFactory" />
    /// </summary>
    /// <param name="options">The SMTP options.</param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="options" /> is <see langword="null"/>.
    /// </exception>
    public SmtpClientFactory(IOptions<MailingOptions> options) {
        _options = Prevent.Argument.Null(options);
    }

    /// <inhertidoc />
    public async Task<ISmtpClient> CreateAsync(CancellationToken cancellationToken) {
        var client = new SmtpClient();

        await client.ConnectAsync(_options.Value.Host,
                         _options.Value.Port,
                         _options.Value.SecureSocket,
                         cancellationToken)
                    .ConfigureAwait(false);

        await client.AuthenticateSmtpClientAsync(_options.Value, cancellationToken)
                    .ConfigureAwait(false);

        return client;
    }
}