using Microsoft.Extensions.Logging;
using MimeKit;
using Nameless.Mailing.MailKit.Internals;

namespace Nameless.Mailing.MailKit;

public sealed class MailingService : IMailingService {
    private readonly ISmtpClientFactory _smtpClientFactory;
    private readonly ILogger<MailingService> _logger;

    public MailingService(ISmtpClientFactory smtpClientFactory, ILogger<MailingService> logger) {
        _smtpClientFactory = Prevent.Argument.Null(smtpClientFactory);
        _logger = Prevent.Argument.Null(logger);
    }

    public async Task SendAsync(Message message, CancellationToken cancellationToken) {
        Prevent.Argument.Null(message);

        var mail = CreateMimeMessage(message);

        if (mail.From.Count == 0) {
            throw new InvalidOperationException("Sender address not found.");
        }

        if (mail.To.Count == 0) {
            throw new InvalidOperationException("Recipient address not found.");
        }

        using var client = await _smtpClientFactory.CreateAsync(cancellationToken)
                                                   .ConfigureAwait(continueOnCapturedContext: false);

        try {
            var result = await client.SendAsync(mail, cancellationToken: cancellationToken)
                                     .ConfigureAwait(continueOnCapturedContext: false);

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