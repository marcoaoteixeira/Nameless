using Microsoft.AspNetCore.Builder;
using Nameless.Registration;
using Nameless.Validation.FluentValidation;

namespace Nameless.Web.Hosting.Configs;

public static class ValidatorConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures Validation feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureValidator(WebHostSettings settings) {
            if (settings.DisableValidator) { return self; }

            self.Services.RegisterValidator(
                AssemblyScanAwareHelper.Join(
                    settings.ConfigureValidation,
                    settings.Assemblies
                )
            );

            return self;
        }
    }
}