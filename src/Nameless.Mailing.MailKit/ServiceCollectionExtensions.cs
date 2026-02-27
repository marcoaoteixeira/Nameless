using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.Mailing.MailKit;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to register mailing services.
/// </summary>
public static class ServiceCollectionExtensions {
    private const string SMTP_CLIENT_FACTORY_KEY =
        $"{nameof(ISmtpClientFactory)} :: f90b567c-82d0-4ca0-8040-9dac0162fc29";

    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers the mailing services with the specified
        ///     configuration.
        /// </summary>
        /// <param name="configure">
        ///     The configuration action.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterMailing(Action<MailingOptions>? configure = null) {
            return self.Configure(configure ?? (_ => { }))
                       .InnerRegisterMailing();
        }

        /// <summary>
        ///     Registers the mailing services with the specified
        ///     configuration.
        /// </summary>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterMailing(IConfiguration configuration) {
            var section = configuration.GetSection(nameof(MailingOptions));

            return self.Configure<MailingOptions>(section)
                       .InnerRegisterMailing();
        }

        private IServiceCollection InnerRegisterMailing() {
            self.TryAddKeyedSingleton<ISmtpClientFactory, SmtpClientFactory>(SMTP_CLIENT_FACTORY_KEY);
            self.TryAddSingleton(ResolveMailing);

            return self;
        }
    }

    private static MailingImpl ResolveMailing(IServiceProvider provider) {
        var smtpClientFactory = provider.GetRequiredKeyedService<ISmtpClientFactory>(SMTP_CLIENT_FACTORY_KEY);
        var logger = provider.GetLogger<MailingImpl>();

        return new MailingImpl(smtpClientFactory, logger);
    }
}