using Microsoft.AspNetCore.Builder;
using Nameless.Validation.FluentValidation;

namespace Nameless.Web.Hosting.Configs;

public static class ValidationConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures Validation feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureValidation(WebHostSettings settings) {
            if (settings.DisableValidation) { return self; }

            self.Services.RegisterValidation(
                WebHostSettingsHelper.Join(
                    settings.ValidationRegistrationConfiguration,
                    settings.Assemblies
                )
            );

            return self;
        }
    }
}