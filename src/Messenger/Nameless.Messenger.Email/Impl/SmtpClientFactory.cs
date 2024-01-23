using MailKit.Net.Smtp;

namespace Nameless.Messenger.Email.Impl {
    public sealed class SmtpClientFactory : ISmtpClientFactory {
        #region Private Read-Only Fields

        private readonly MessengerOptions _options;

        #endregion

        #region Public Constructors

        public SmtpClientFactory()
            : this(MessengerOptions.Default) { }

        public SmtpClientFactory(MessengerOptions options) {
            _options = Guard.Against.Null(options, nameof(options));
        }

        #endregion

        #region SmtpClientFactory Members

        public async Task<ISmtpClient> CreateAsync(CancellationToken cancellationToken = default) {
            var client = new SmtpClient();

            await client.ConnectAsync(
                _options.Host,
                _options.Port,
                _options.UseSsl,
                cancellationToken
            );

            // Authenticate if possible and needed.
            if (_options.UseCredentials() && client.Capabilities.HasFlag(SmtpCapabilities.Authentication)) {
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(
                    _options.Username,
                    _options.Password,
                    cancellationToken
                );
            }

            return client;
        }

        #endregion
    }
}
