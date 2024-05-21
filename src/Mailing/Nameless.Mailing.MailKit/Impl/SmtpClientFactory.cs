using MailKit.Net.Smtp;
using Nameless.Mailing.MailKit.Options;

namespace Nameless.Mailing.MailKit.Impl {
    public sealed class SmtpClientFactory : ISmtpClientFactory {
        #region Private Read-Only Fields

        private readonly ServerOptions _options;

        #endregion

        #region Public Constructors

        public SmtpClientFactory()
            : this(ServerOptions.Default) { }

        public SmtpClientFactory(ServerOptions options) {
            _options = Guard.Against.Null(options, nameof(options));
        }

        #endregion

        #region Private Static Methods

        private static async Task<ISmtpClient> AuthenticateSmtpClient(ISmtpClient client, ServerOptions serverOptions, CancellationToken cancellationToken) {
            client.AuthenticationMechanisms.Remove("XOAUTH2");

            await client.AuthenticateAsync(serverOptions.Username,
                                           serverOptions.Password,
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

            return client.Capabilities.HasFlag(SmtpCapabilities.Authentication) && _options.UseCredentials()
                ? await AuthenticateSmtpClient(client, _options, cancellationToken)
                : client;
        }

        #endregion
    }
}
