using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.Mailing.Mailkit;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to register mailing services.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    extension(IServiceCollection self) {
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
        public IServiceCollection RegisterMailing(IConfiguration? configuration = null) {
            self.ConfigureOptions<MailingOptions>(configuration);

            self.TryAddSingleton<ISmtpClientFactory, SmtpClientFactory>();
            self.TryAddSingleton<IMailingService, MailingService>();

            return self;
        }
    }
}