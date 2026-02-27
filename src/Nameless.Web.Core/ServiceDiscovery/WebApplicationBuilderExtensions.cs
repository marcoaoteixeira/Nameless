using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Helpers;

namespace Nameless.Web.ServiceDiscovery;

/// <summary>
///     Host application builder extension methods.
/// </summary>
public static class WebApplicationBuilderExtensions {
    /// <param name="self">
    ///     The current <see cref="WebApplicationBuilder"/>.
    /// </param>
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Registers discovery services in the application builder.
        /// </summary>
        /// <param name="registration">
        ///     The registration settings delegate.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WebApplicationBuilder RegisterServiceDiscovery(Action<ServiceDiscoveryRegistrationSettings> registration) {
            var settings = ActionHelper.FromDelegate(registration);

            self.Services
                .AddServiceDiscovery(settings.ConfigureServiceDiscovery)
                .ConfigureHttpClientDefaults(http => {
                    // Turn on resilience by default
                    http.AddStandardResilienceHandler(settings.ConfigureStandardResilienceHandler);

                    // Turn on service discovery by default
                    http.AddServiceDiscovery();
                });

            return self;
        }
    }
}