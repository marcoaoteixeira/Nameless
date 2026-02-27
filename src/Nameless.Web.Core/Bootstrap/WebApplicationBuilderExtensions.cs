using Microsoft.AspNetCore.Builder;
using Nameless.Bootstrap;
using Nameless.Bootstrap.Infrastructure;

namespace Nameless.Web.Bootstrap;

/// <summary>
///     <see cref="WebApplicationBuilder"/> extension methods.
/// </summary>
public static class WebApplicationBuilderExtensions {
    /// <param name="self">
    ///     The current <see cref="WebApplicationBuilder"/>.
    /// </param>
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Registers Bootstrap services.
        /// </summary>
        /// <param name="registration">
        ///     The Bootstrap registration settings delegate.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder RegisterBootstrap(Action<BootstrapRegistrationSettings> registration) {
            self.Services.RegisterBootstrap(registration, self.Configuration);

            return self;
        }

        /// <summary>
        ///     Registers HTTP context correlation services.
        /// </summary>
        /// <param name="registration">
        ///     The Bootstrap registration settings delegate.
        /// </param>
        /// <param name="configure">
        ///     The configuration delegate.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder RegisterBootstrap(Action<BootstrapRegistrationSettings> registration, Action<BootstrapOptions> configure) {
            self.Services.RegisterBootstrap(registration, configure);

            return self;
        }
    }
}