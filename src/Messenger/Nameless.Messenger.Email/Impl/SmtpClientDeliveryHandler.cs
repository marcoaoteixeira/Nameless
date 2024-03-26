using MimeKit;

namespace Nameless.Messenger.Email.Impl {
    public sealed class SmtpClientDeliveryHandler : IDeliveryHandler {
        #region Private Read-Only Fields

        private readonly ISmtpClientFactory _smtpClientFactory;

        #endregion

        #region Public Constructors

        public SmtpClientDeliveryHandler(ISmtpClientFactory smtpClientFactory) {
            _smtpClientFactory = Guard.Against.Null(smtpClientFactory, nameof(smtpClientFactory));
        }

        #endregion

        #region IDeliveryHandler Members

        public DeliveryMode Mode => DeliveryMode.Network;

        public async Task<string> HandleAsync(MimeMessage message, CancellationToken cancellationToken = default) {
            using var client = await _smtpClientFactory.CreateAsync(cancellationToken);

            var result = await client.SendAsync(message, cancellationToken: cancellationToken);
            await client.DisconnectAsync(quit: true, cancellationToken);

            return result;
        }

        #endregion
    }
}
