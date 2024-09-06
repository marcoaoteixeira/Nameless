using Microsoft.Extensions.Logging;
using MimeKit;
using Nameless.Mailing.MailKit.Internals;

namespace Nameless.Mailing.MailKit.Impl;

public sealed class MailingService : IMailingService {
    private readonly ISmtpClientFactory _smtpClientFactory;
    private readonly ILogger _logger;

    public MailingService(ISmtpClientFactory smtpClientFactory, ILogger<MailingService> logger) {
        _smtpClientFactory = Prevent.Argument.Null(smtpClientFactory, nameof(smtpClientFactory));
        _logger = Prevent.Argument.Null(logger, nameof(logger));
    }

    public async Task<string> DispatchAsync(Message message, CancellationToken cancellationToken) {
        Prevent.Argument.Null(message, nameof(message));

        if (message.From.IsNullOrEmpty()) {
            throw new InvalidOperationException("Missing sender address.");
        }

        if (message.To.IsNullOrEmpty()) {
            throw new InvalidOperationException("Missing recipient address.");
        }

        var mail = CreateMimeMessage(message);

        using var client = await _smtpClientFactory.CreateAsync(cancellationToken);

        string result;
        try { result = await client.SendAsync(mail, cancellationToken: cancellationToken); }
        catch (Exception ex) {
            LoggerHelper.SendMessageFailure(_logger, ex);
            result = ex.Message;
        }
        return result;
    }

    private static MimeMessage CreateMimeMessage(Message message) {
        var mail = message.ToMimeMessage();

        AddressHelper.SetRecipients(mail.From, message.From);
        AddressHelper.SetRecipients(mail.To, message.To);
        AddressHelper.SetRecipients(mail.Cc, AddressHelper.SplitAddresses(message.Parameters.GetCarbonCopy()));
        AddressHelper.SetRecipients(mail.Bcc, AddressHelper.SplitAddresses(message.Parameters.GetBlindCarbonCopy()));

        return mail;
    }
}