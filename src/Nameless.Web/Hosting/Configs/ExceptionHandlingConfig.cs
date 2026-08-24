using Microsoft.AspNetCore.Builder;
using Nameless.Helpers;
using Nameless.Web.ErrorHandling;

namespace Nameless.Web.Hosting.Configs;

public static class ExceptionHandlingConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures ExceptionHandling feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureExceptionHandling(WebHostSettings settings) {
            if (settings.DisableExceptionHandling) { return self; }

            self.Services.RegisterErrorHandling(
                settings.ConfigureExceptionHandling
            );

            return self;
        }
    }

    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses the exception handling service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseExceptionHandling(WebHostSettings settings) {
            if (settings.DisableExceptionHandling) { return self; }

            var registration = ActionHelper.FromDelegate(settings.ConfigureExceptionHandling);
            var opts = ActionHelper.FromDelegate(registration.ConfigureExceptionHandlerOptions);

            self.UseExceptionHandler(opts);

            return self;
        }
    }
}

