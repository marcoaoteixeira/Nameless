using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Mailing.MailKit;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to register mailing services.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// Registers the mailing services with the specified configuration.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/>.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/> so other actions can be chained.
    /// </returns>
    public static IServiceCollection ConfigureMailingServices(this IServiceCollection self, Action<MailingOptions>? configure = null) {
        return self.Configure(configure ?? (_ => { }))
                   .RegisterMainServices();
    }

    private static IServiceCollection RegisterMainServices(this IServiceCollection self) {
        return self.AddSingleton<ISmtpClientFactory, SmtpClientFactory>()
                   .AddSingleton<IMailingService, MailingService>();
    }
}