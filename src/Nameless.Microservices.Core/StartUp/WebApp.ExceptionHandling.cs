using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Microservices.Infrastructure.ErrorHandling;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures ExceptionHandling feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ExceptionHandlingConfig(WebAppSettings settings) {
            if (settings.DisableExceptionHandling) { return self; }

            self.Services.AddExceptionHandler<GlobalExceptionHandler>();

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
        ///     The <see cref="WebAppSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseExceptionHandling(WebAppSettings settings) {
            if (settings.DisableExceptionHandling) { return self; }

            self.UseExceptionHandler();

            return self;
        }
    }
}

