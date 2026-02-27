using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures the authentication, authorization and JWT bearer,
        ///     if enabled.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder AntiforgeryConfig(WebAppSettings settings) {
            if (settings.DisableAntiforgery) { return self; }

            self.Services.AddAntiforgery(opts => {
                opts.HeaderName = "X-CSRF-TOKEN";
                opts.Cookie.Name = "X-CSRF-TOKEN";
                opts.Cookie.HttpOnly = true;
                opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            return self;
        }
    }

    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses the antiforgery service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebAppSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseAntiforgery(WebAppSettings settings) {
            if (settings.DisableAntiforgery) { return self; }

            self.UseAntiforgery();

            return self;
        }
    }
}
