using Microsoft.Extensions.DependencyInjection;
using Nameless.Mailing.MailKit.Impl;
using Nameless.Mailing.MailKit.Options;

namespace Nameless.Mailing.MailKit;

public static class ServiceCollectionExtension {
    public static IServiceCollection AddMailing(this IServiceCollection self, Action<MailServerOptions>? configure = null)
        => self.AddSingleton<IMailingService>(provider => {
            var options = provider.GetOptions<MailServerOptions>();

            configure?.Invoke(options.Value);

            return new MailingService(
                smtpClientFactory: new SmtpClientFactory(options),
                logger: provider.GetLogger<MailingService>()
            );
        });
}