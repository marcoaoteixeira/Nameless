using Microsoft.Extensions.DependencyInjection;
using Nameless.Mailing.MailKit.Impl;
using Nameless.Mailing.MailKit.Options;

namespace Nameless.Mailing.MailKit {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection RegisterMailingService(this IServiceCollection self, Action<ServerOptions>? configure = null)
            => self.AddSingleton<IMailingService>(provider => {
                var options = provider.GetPocoOptions<ServerOptions>();

                configure?.Invoke(options);

                return new MailingService(
                    smtpClientFactory: new SmtpClientFactory(options),
                    logger: provider.GetLogger<MailingService>()
                );
            });

        #endregion
    }
}
