using Microsoft.Extensions.DependencyInjection;
using Nameless.Helpers;

namespace Nameless.Web.ServiceDiscovery;

/// <summary>
///     Host application builder extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers discovery services in the application builder.
        /// </summary>
        /// <param name="registration">
        ///     The registration settings delegate.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public IServiceCollection RegisterServiceDiscovery(Action<ServiceDiscoveryRegistration>? registration = null) {
            var settings = ActionHelper.FromDelegate(registration);

            self.AddServiceDiscovery(settings.ConfigureServiceDiscovery)
                .ConfigureHttpClientDefaults(http => {
                    // Turn on resilience by default
                    http.AddStandardResilienceHandler(
                        settings.ConfigureStandardResilienceHandler
                    );

                    // Turn on service discovery by default
                    http.AddServiceDiscovery();
                });

            return self;
        }
    }
}