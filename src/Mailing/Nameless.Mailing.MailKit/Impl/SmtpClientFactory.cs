using MailKit.Net.Smtp;
using Nameless.Mailing.MailKit.Options;

namespace Nameless.Mailing.MailKit.Impl {
    public sealed class SmtpClientFactory : ISmtpClientFactory {
        #region Private Read-Only Fields

        private readonly MailServerOptions _options;

        #endregion

        #region Public Constructors

        public SmtpClientFactory()
            : this(MailServerOptions.Default) { }

        public SmtpClientFactory(MailServerOptions options) {
            _options = Guard.Against.Null(options, nameof(options));
        }

        #endregion

        #region Private Static Methods

        private static async Task<ISmtpClient> AuthenticateSmtpClient(SmtpClient client, MailServerOptions mailServerOptions, CancellationToken cancellationToken) {
            client.AuthenticationMechanisms.Remove("XOAUTH2");

            await client.AuthenticateAsync(mailServerOptions.Username,
                                           mailServerOptions.Password,
                                           cancellationToken);

            return client;
        }

        #endregion

        #region SmtpClientFactory Members

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

        #endregion
    }
}
