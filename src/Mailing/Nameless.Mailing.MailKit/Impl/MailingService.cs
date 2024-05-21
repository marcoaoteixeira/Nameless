using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Nameless.Mailing.MailKit.Impl {
    public sealed class MailingService : IMailingService {
        #region Private Read-Only Fields

        private readonly ISmtpClientFactory _smtpClientFactory;
        private readonly ILogger _logger;

        #endregion

        #region Public Constructors

        public MailingService(ISmtpClientFactory smtpClientFactory, ILogger<MailingService> logger) {
            _smtpClientFactory = Guard.Against.Null(smtpClientFactory, nameof(smtpClientFactory));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        #endregion

        #region Private Static Methods

        private static MimeMessage CreateMimeMessage(Message message) {
            var mail = message.ToMimeMessage();

            Internals.SetRecipients(mail.From, message.From);
            Internals.SetRecipients(mail.To, message.To);
            Internals.SetRecipients(mail.Cc, Internals.SplitAddresses(message.Parameters.GetCarbonCopy()));
            Internals.SetRecipients(mail.Bcc, Internals.SplitAddresses(message.Parameters.GetBlindCarbonCopy()));

            return mail;
        }

        #endregion

        #region IMailingService Members

        public async Task<string> DispatchAsync(Message message, CancellationToken cancellationToken) {
            Guard.Against.Null(message, nameof(message));

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
                _logger.LogError(ex, "Error while sending mail. Message: {Message}", ex.Message);
                result = ex.Message;
            }
            return result;
        }

        #endregion
    }
}
