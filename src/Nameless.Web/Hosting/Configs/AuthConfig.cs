using Microsoft.AspNetCore.Builder;
using Nameless.Web.Auth;

namespace Nameless.Web.Hosting.Configs;

public static class AuthConfig {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder ConfigureAuth(WebHostSettings settings) {
            if (settings.DisableAuth) { return self; }

            self.Services.RegisterAuth(
                settings.AuthRegistrationConfiguration,
                self.Configuration
            );

            return self;
        }
    }

    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses the authentication/authorization service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseAuth(WebHostSettings settings) {
            if (settings.DisableAuth) { return self; }

            self.UseAuthentication()
                .UseAuthorization();

            return self;
        }
    }
}
