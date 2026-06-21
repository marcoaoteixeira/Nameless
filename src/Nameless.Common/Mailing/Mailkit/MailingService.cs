using Microsoft.Extensions.Logging;
using MimeKit;

namespace Nameless.Mailing.Mailkit;

/// <summary>
/// Default implementation of <see cref="IMailingService"/> using MailKit for sending emails.
/// </summary>
public class MailingService : IMailingService {
    private readonly ILogger<MailingService> _logger;
    private readonly ISmtpClientFactory _smtpClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MailingService"/> class.
    /// </summary>
    /// <param name="smtpClientFactory">The SMTP client factory.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="smtpClientFactory"/> or
    ///     <paramref name="logger"/> is <see langword="null"/>.
    /// </exception>
    public MailingService(ISmtpClientFactory smtpClientFactory, ILogger<MailingService> logger) {
        _smtpClientFactory = smtpClientFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="message"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="Message.From"/> or
    ///     <see cref="Message.To"/> is empty.
    /// </exception>
    public async Task DeliverAsync(Message message, CancellationToken cancellationToken) {
        var mail = CreateMimeMessage(message);

        if (mail.From.Count == 0) {
            throw new InvalidOperationException("Sender address not found.");
        }

        if (mail.To.Count == 0) {
            throw new InvalidOperationException("Recipient address not found.");
        }

        using var client = await _smtpClientFactory.CreateAsync(cancellationToken)
                                                   .SkipContextSync();

        try {
            var result = await client.SendAsync(mail, cancellationToken)
                                     .SkipContextSync();

            _logger.DeliverResult(result);
        }
        catch (Exception ex) {
            _logger.DeliverFailure(ex);

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