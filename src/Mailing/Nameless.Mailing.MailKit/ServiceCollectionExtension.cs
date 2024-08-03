using Microsoft.Extensions.DependencyInjection;
using Nameless.Mailing.MailKit.Impl;
using Nameless.Mailing.MailKit.Options;

namespace Nameless.Mailing.MailKit {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection RegisterMailingService(this IServiceCollection self, Action<MailServerOptions>? configure = null)
            => self.AddSingleton<IMailingService>(provider => {
                var options = provider.GetOptions<MailServerOptions>();

                configure?.Invoke(options.Value);

                return new MailingService(
                    smtpClientFactory: new SmtpClientFactory(options.Value),
                    logger: provider.GetLogger<MailingService>()
                );
            });

        #endregion
    }
}
