using Microsoft.Extensions.Logging;
using MimeKit;
using Nameless.Mailing.MailKit.Internals;

namespace Nameless.Mailing.MailKit;

/// <summary>
/// Default implementation of <see cref="IMailingService"/> using MailKit for sending emails.
/// </summary>
public sealed class MailingService : IMailingService {
    private readonly ILogger<MailingService> _logger;
    private readonly ISmtpClientFactory _smtpClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MailingService"/> class.
    /// </summary>
    /// <param name="smtpClientFactory">The SMTP client factory.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="smtpClientFactory"/> or
    ///     <paramref name="logger"/> is <c>null</c>.
    /// </exception>
    public MailingService(ISmtpClientFactory smtpClientFactory, ILogger<MailingService> logger) {
        _smtpClientFactory = Prevent.Argument.Null(smtpClientFactory);
        _logger = Prevent.Argument.Null(logger);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="message"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="Message.From"/> or
    ///     <see cref="Message.To"/> is empty.
    /// </exception>
    public async Task DeliverAsync(Message message, CancellationToken cancellationToken) {
        Prevent.Argument.Null(message);

        var mail = CreateMimeMessage(message);

        if (mail.From.Count == 0) {
            throw new InvalidOperationException("Sender address not found.");
        }

        if (mail.To.Count == 0) {
            throw new InvalidOperationException("Recipient address not found.");
        }

        using var client = await _smtpClientFactory.CreateAsync(cancellationToken)
                                                   .ConfigureAwait(false);

        try {
            var result = await client.SendAsync(mail, cancellationToken)
                                     .ConfigureAwait(false);

            _logger.EmailSentResult(result);
        }
        catch (Exception ex) {
            _logger.SendMessageError(ex);

            throw;
        }
    }

    private static MimeMessage CreateMimeMessage(Message message) {
        var mail = message.ToMimeMessage();

        AddressHelper.SetRecipients(mail.From, message.From);
        AddressHelper.SetRecipients(mail.To, message.To);
        AddressHelper.SetRecipients(mail.Cc, message.Cc);
        AddressHelper.SetRecipients(mail.Bcc, message.Bcc);

        return mail;
    }
}